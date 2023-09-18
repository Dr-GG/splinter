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
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.ServiceScope;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Default.NanoWaveFunctions;

/// <summary>
/// The default implementation of the ISuperpositionNanoWaveFunction interface.
/// </summary>
public class SuperpositionNanoWaveFunction : ISuperpositionNanoWaveFunction
{
    private ITeraAgent? _parent;

    private readonly SuperpositionSettings _settings;
    private readonly ISuperpositionMappingRegistry _mappingRegistry;
    private readonly ISuperpositionSingletonRegistry _singletonRegistry;

    private INanoWaveFunctionContainer? _nanoWaveFunctionContainer;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SuperpositionNanoWaveFunction(
        SuperpositionSettings settings,
        ISuperpositionMappingRegistry mappingRegistry,
        ISuperpositionSingletonRegistry singletonRegistry)
    {
        _settings = settings;
        _mappingRegistry = mappingRegistry;
        _singletonRegistry = singletonRegistry;
    }

    private ITeraAgent Parent => _parent.AssertNanoTypeReturnGetterValue();

    private INanoWaveFunctionContainer NanoWaveFunctionContainer => _nanoWaveFunctionContainer.AssertNanoServiceReturnGetterValue();

    /// <inheritdoc />
    public async Task Initialise(ITeraAgent parent, IEnumerable<SuperpositionMapping> superpositionMappings)
    {
        _parent = parent;

        await using var scope = await Parent.Scope.Start();
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
        await using var scope = await Parent.Scope.Start();
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
            await RegisterNanoTypeDependency(scope, parameters.TeraAgentId.Value, nanoAgent.TypeId);
        }

        return nanoAgent;
    }

    private static async Task RegisterNanoTypeDependency(
        IServiceScope scope,
        Guid teraAgentId,
        SplinterId nanoTypeId)
    {
        var dependencyService = await scope.Resolve<ITeraAgentNanoTypeDependencyService>();

        await dependencyService.IncrementTeraAgentNanoTypeDependencies(teraAgentId, nanoTypeId);
    }

    /// <inheritdoc />
    public async Task<Guid?> Recollapse(NanoRecollapseParameters parameters)
    {
        var recollapseParameters = parameters.Cast<NanoSuperpositionMappingRecollapseParameters>();
        var originalMapping = await GetRecollapseMapping(recollapseParameters);

        if (originalMapping == null)
        {
            return null;
        }

        var newMapping = recollapseParameters.SuperpositionMapping;
        var newInternalMapping = ToNewInternalMapping(originalMapping, newMapping);
        var sourceTeraId = parameters.SourceTeraId ?? Parent.TeraId;

        await _mappingRegistry.Synch(newInternalMapping);
        await SynchContainer(newInternalMapping.NanoTypeId, newInternalMapping.NanoInstanceType);

        return await SignalNanoTypeRecollapses(sourceTeraId, originalMapping.NanoTypeId, recollapseParameters.TeraIds);
    }

    public async ValueTask DisposeAsync()
    {
        await _singletonRegistry.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    private async Task SynchContainer(Guid nanoTypeId, string nanoInstanceType)
    {
        var newCollapseType = Type.GetType(nanoInstanceType);

        if (newCollapseType == null)
        {
            throw new InvalidNanoInstanceException(
                $"Could not find the nano instance type {nanoInstanceType}.");
        }

        await NanoWaveFunctionContainer.Synch(nanoTypeId, newCollapseType);
    }

    private async Task<Guid?> SignalNanoTypeRecollapses(Guid sourceTeraId, Guid nanoTypeId, IEnumerable<Guid>? teraIds)
    {
        await using var scope = await Parent.Scope.Start();
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
        return await NanoWaveFunctionContainer.Collapse(collapseParameters);
    }

    private async Task<INanoAgent?> CollapseSingleton(NanoCollapseParameters collapseParameters)
    {
        var singleton = await _singletonRegistry.Fetch(collapseParameters.NanoTypeId);

        if (singleton != null)
        {
            return singleton;
        }

        singleton = await NanoWaveFunctionContainer.Collapse(collapseParameters);

        if (singleton == null)
        {
            return null;
        }

        return await _singletonRegistry.Register(collapseParameters.NanoTypeId, singleton);
    }
}