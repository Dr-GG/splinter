using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Interfaces.NanoWaveFunctions;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Models;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Interfaces.WaveFunctions;
using Splinter.NanoTypes.Default.Domain.Enums;
using Splinter.NanoTypes.Default.Domain.Exceptions.Superposition;
using Splinter.NanoTypes.Default.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Default.NanoWaveFunctions;

/// <summary>
/// The default implementation of the ISuperpositionNanoWaveFunction interface.
/// </summary>
public class SuperpositionNanoWaveFunction : ISuperpositionNanoWaveFunction
{
    private readonly SuperpositionSettings _settings;
    private readonly ITeraAgent _parent;
    private readonly ISuperpositionMappingRegistry _mappingRegistry;
    private readonly ISuperpositionSingletonRegistry _singletonRegistry;

    private INanoWaveFunctionContainer? _nanoWaveFunctionContainer;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SuperpositionNanoWaveFunction(
        SuperpositionSettings settings,
        ITeraAgent parent,
        ISuperpositionMappingRegistry mappingRegistry, 
        ISuperpositionSingletonRegistry singletonRegistry)
    {
        _settings = settings;
        _parent = parent;
        _mappingRegistry = mappingRegistry;
        _singletonRegistry = singletonRegistry;
    }

    /// <inheritdoc />
    public async Task Initialise(IEnumerable<SuperpositionMapping> superpositionMappings)
    {
        await using var scope = await _parent.Scope.Start();
        var resolver = await scope.Resolve<ISuperpositionMappingResolver>();
        var waveBuilder = await scope.Resolve<INanoWaveFunctionBuilder>();
        var mappings = (await resolver.Resolve(superpositionMappings)).ToArray();

        await _mappingRegistry.Register(mappings);

        foreach (var mapping in mappings)
        {
            await waveBuilder.Register(mapping.NanoInstanceType);
        }

        _nanoWaveFunctionContainer = await waveBuilder.Build(_settings.RegistryTimeoutSpan);
    }

    /// <inheritdoc />
    public async Task<INanoAgent?> Collapse(NanoCollapseParameters parameters)
    {
        if (_nanoWaveFunctionContainer == null)
        {
            return null;
        }

        await using var scope = await _parent.Scope.Start();
        var dependencyService = await scope.Resolve<ITeraAgentNanoTypeDependencyService>();
        var mapping = await _mappingRegistry.Fetch(parameters.NanoTypeId);

        if (mapping == null)
        {
            return null;
        }

        var nanoAgent = await CollapseNanoAgent(mapping, parameters);

        if (mapping.Mode == SuperpositionMode.Recollapse
            && parameters.TeraAgentId.HasValue
            && nanoAgent != null)
        {
            await dependencyService
                .IncrementTeraAgentNanoTypeDependencies(
                    parameters.TeraAgentId.Value, mapping.NanoTypeId.ToSplinterId());
        }

        return nanoAgent;
    }

    /// <inheritdoc />
    public async Task<Guid?> Recollapse(NanoRecollapseParameters parameters)
    {
        if (_nanoWaveFunctionContainer == null)
        {
            return null;
        }

        var recollapseParameters = parameters.Cast<NanoSuperpositionMappingRecollapseParameters>();
        var originalMapping = await GetRecollapseMapping(recollapseParameters);

        if (originalMapping == null)
        {
            return null;
        }

        var newMapping = recollapseParameters.SuperpositionMapping;
        var newInternalMapping = ToNewInternalMapping(originalMapping, newMapping);
        var sourceTeraId = parameters.SourceTeraId ?? _parent.TeraId;

        await _mappingRegistry.Synch(newInternalMapping);
        await SynchContainer(newInternalMapping.NanoTypeId, newInternalMapping.NanoInstanceType);

        return await SignalNanoTypeRecollapses(sourceTeraId, originalMapping.NanoTypeId, recollapseParameters.TeraIds);
    }

    private async Task SynchContainer(Guid nanoTypeId, string nanoInstanceType)
    {
        if (_nanoWaveFunctionContainer == null)
        {
            return;
        }

        var newCollapseType = Type.GetType(nanoInstanceType);

        if (newCollapseType == null)
        {
            throw new InvalidNanoInstanceException(
                $"Could not find the nano instance type {newCollapseType}.");
        }

        await _nanoWaveFunctionContainer.Synch(nanoTypeId, newCollapseType);
    }

    private async Task<Guid?> SignalNanoTypeRecollapses(Guid sourceTeraId, Guid nanoTypeId, IEnumerable<Guid>? teraIds)
    {
        await using var scope = await _parent.Scope.Start();
        var dependencyService = await scope.Resolve<ITeraAgentNanoTypeDependencyService>();

        return await dependencyService.SignalNanoTypeRecollapses(sourceTeraId, nanoTypeId, teraIds);
    }

    private static InternalSuperpositionMapping ToNewInternalMapping(
        InternalSuperpositionMapping originalMapping,
        SuperpositionMapping newMapping)
    {
        return new InternalSuperpositionMapping
        {
            NanoTypeId = originalMapping.NanoTypeId,
            Mode = originalMapping.Mode,
            Scope = newMapping.Scope,
            NanoInstanceType = newMapping.NanoInstanceType,
            Description = newMapping.Description.IsNullOrEmpty()
                ? originalMapping.Description
                : newMapping.Description

        };
    }

    private async Task<InternalSuperpositionMapping?> GetRecollapseMapping(
        NanoSuperpositionMappingRecollapseParameters recollapseParameters)
    {
        var mapping = await _mappingRegistry.Fetch(recollapseParameters.NanoTypeId);

        if (mapping == null)
        {
            return null;
        }

        if (mapping.Mode != SuperpositionMode.Recollapse)
        {
            throw new InvalidNanoTypeRecollapseOperationException(
                $"The superposition mapping {mapping.NanoTypeId} does not support recollapse.");
        }

        if (mapping.Scope == SuperpositionScope.Singleton)
        {
            throw new InvalidNanoTypeRecollapseOperationException(
                "The recollapse of singletons are not supported.");
        }

        return mapping;
    }

    public async ValueTask DisposeAsync()
    {
        await _singletonRegistry.DisposeAsync();
    }

    private async Task<INanoAgent?> CollapseNanoAgent(
        SuperpositionMapping mapping,
        NanoCollapseParameters parameters)
    {
        return mapping.Scope switch
        {
            SuperpositionScope.Request => await CollapseRequest(parameters),
            SuperpositionScope.Singleton => await CollapseSingleton(parameters),
            _ => throw new NotSupportedException($"The scope {mapping.Scope} is not supported.")
        };
    }

    private async Task<INanoAgent?> CollapseRequest(NanoCollapseParameters collapseParameters)
    {
        if (_nanoWaveFunctionContainer == null)
        {
            return null;
        }

        return await _nanoWaveFunctionContainer.Collapse(collapseParameters);
    }

    private async Task<INanoAgent?> CollapseSingleton(NanoCollapseParameters collapseParameters)
    {
        if (_nanoWaveFunctionContainer == null)
        {
            return null;
        }

        var singleton = await _singletonRegistry.Fetch(collapseParameters.NanoTypeId);

        if (singleton != null)
        {
            return singleton;
        }

        singleton = await _nanoWaveFunctionContainer.Collapse(collapseParameters);

        if (singleton == null)
        {
            return null;
        }

        return await _singletonRegistry.Register(collapseParameters.NanoTypeId, singleton);
    }
}