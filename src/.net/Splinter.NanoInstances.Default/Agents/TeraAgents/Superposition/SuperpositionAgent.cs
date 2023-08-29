using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoInstances.Default.Interfaces.NanoWaveFunctions;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.NanoWaveFunctions;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Default.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;

namespace Splinter.NanoInstances.Default.Agents.TeraAgents.Superposition;

/// <summary>
/// The default implementation of the ISuperpositionAgent interface.
/// </summary>
public class SuperpositionAgent : TeraAgent, ISuperpositionAgent
{
    /// <summary>
    /// The Tera Type ID.
    /// </summary>
    public override SplinterId TypeId => SplinterIdConstants.SuperpositionAgentNanoTypeId;

    /// <summary>
    /// The Tera Instance ID.
    /// </summary>
    public override SplinterId InstanceId => new()
    {
        Name = "Default Superposition Agent",
        Version = "1.0.0",
        Guid = new Guid("{F253ADF7-6295-47B1-9DE2-5C7980AB125C}")
    };

    /// <inheritdoc />
    protected override bool RegisterInContainer => false;

    /// <inheritdoc />
    protected override bool HasKnowledge => false;

    /// <inheritdoc />
    public override async Task Initialise(NanoInitialisationParameters parameters)
    {
        var initParameters = parameters.Cast<SuperpositionInitialisationParameters>();

        Scope = initParameters.ServiceScope;

        await InitialiseSuperpositionWaveFunction(initParameters.SuperpositionMappings);
    }

    /// <inheritdoc />
    public override async Task<INanoAgent?> Collapse(NanoCollapseParameters parameters)
    {
        var nanoAgent = await NanoWaveFunction.Collapse(parameters);

        if (nanoAgent == null)
        {
            return null;
        }

        await RegisterNanoTypes(nanoAgent);

        return nanoAgent;
    }

    /// <inheritdoc />
    public async Task<Guid?> Recollapse(NanoRecollapseParameters parameters)
    {
        if (NanoWaveFunction is not ISuperpositionNanoWaveFunction superpositionNanoWaveFunction)
        {
            return null;
        }

        return await superpositionNanoWaveFunction.Recollapse(parameters);
    }

    /// <inheritdoc />
    public async Task Sync(NanoTypeDependencyTerminationParameters parameters)
    {
        await using var scope = await NewScope();
        var service = await scope.Resolve<ITeraAgentNanoTypeDependencyService>();

        await service.DecrementTeraAgentNanoTypeDependencies(parameters.TeraId, parameters.NanoType);
    }

    /// <inheritdoc />
    public async Task Sync(NanoRecollapseOperationParameters parameters)
    {
        await using var scope = await NewScope();
        var service = await scope.Resolve<INanoTypeRecollapseOperationService>();

        await service.Sync(parameters);
    }

    private async Task InitialiseSuperpositionWaveFunction(
        IEnumerable<SuperpositionMapping> superpositionMappings)
    {
        var settings = await Scope.Resolve<SuperpositionSettings>();
        var mappingRegistry = await Scope.Resolve<ISuperpositionMappingRegistry>();
        var singletonRegistry = await Scope.Resolve<ISuperpositionSingletonRegistry>();
        var waveFunction = new SuperpositionNanoWaveFunction(
            settings, this, mappingRegistry, singletonRegistry);

        await waveFunction.Initialise(superpositionMappings);

        NanoWaveFunction = waveFunction;
    }

    private async Task RegisterNanoTypes(INanoAgent nanoAgent)
    {
        await using var scope = await NewScope();
        var nanoTypeManager = await scope.Resolve<INanoTypeManager>();

        await nanoTypeManager.RegisterNanoType(nanoAgent);
    }
}