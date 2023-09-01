using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Registration;
using Splinter.NanoTypes.Domain.Parameters.Termination;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Agents.TeraAgents;

/// <summary>
/// The default implementation of the ITeraAgent interface.
/// </summary>
public abstract class TeraAgent : NanoAgent, ITeraAgent
{
    private INanoTable? _nanoTable;
    private INanoReference? _knowledgeReference;

    /// <inheritdoc />
    public override bool HasNoTeraParent => true;

    /// <inheritdoc />
    public override bool HasTeraParent => false;

    /// <inheritdoc />
    public override bool HasNoNanoTable => _nanoTable == null;

    /// <inheritdoc />
    public override bool HasNanoTable => _nanoTable != null;

    /// <inheritdoc />
    public override HolonType HolonType => HolonType.Tera;

    /// <inheritdoc />
    public override ITeraAgent TeraParent => this;

    /// <inheritdoc />
    public Guid TeraId { get; protected set; }

    /// <inheritdoc />
    public override INanoTable NanoTable => _nanoTable.AssertNanoServiceReturnGetterValue();

    /// <inheritdoc />
    public ITeraKnowledgeAgent Knowledge => _knowledgeReference.AssertReturnGetterValue
        <INanoReference, NanoTypeNotInitialiseException<ITeraKnowledgeAgent>>().Typed<ITeraKnowledgeAgent>();

    /// <summary>
    /// The flag indicating if the TeraAgent instance should be registered in the ITeraAgentContainer instance.
    /// </summary>
    protected virtual bool RegisterInContainer => true;

    /// <summary>
    /// The flag indicating if the TeraAgent instance has an ITeraKnowledgeAgent instance.
    /// </summary>
    protected virtual bool HasKnowledge => true;

    /// <summary>
    /// The Nano Type ID of the ITeraKnowledgeAgent.
    /// </summary>
    protected virtual SplinterId TeraKnowledgeNanoTypeId => SplinterIdConstants.TeraDefaultKnowledgeNanoTypeId;

    /// <inheritdoc />
    public override async Task Initialise(NanoInitialisationParameters parameters)
    {
        var validParameters = await ValidateServiceScope(parameters);

        await base.Initialise(validParameters);

        await Register(validParameters);
        await InitialiseNanoTable(validParameters);
        await InternalInitialiseNanoReferences(validParameters);
    }

    /// <inheritdoc />
    public async Task RegisterSelf(TeraAgentRegistrationParameters parameters)
    {
        var correctParameters = parameters with {NanoInstanceId = InstanceId};

        await InitialiseNanoTable();
        await InternalInitialiseNanoReferences();

        TeraId = await TeraRegistryAgent.Register(correctParameters);
    }

    /// <inheritdoc />
    public virtual async Task Execute(TeraAgentExecutionParameters parameters)
    {
        if (_knowledgeReference == null || _knowledgeReference.HasNoReference)
        {
            return;
        }

        await _knowledgeReference.Typed<ITeraKnowledgeAgent>().Execute(parameters);
    }

    /// <inheritdoc />
    public override async Task<INanoAgent?> Collapse(NanoCollapseParameters parameters)
    {
        var agent = await NanoWaveFunction.Collapse(parameters);

        if (await InitialiseNanoAgent(parameters, agent))
        {
            return agent;
        }

        if (SplinterEnvironment.Status != SplinterEnvironmentStatus.Initialised)
        {
            return null;
        }

        agent = await SplinterEnvironment.SuperpositionAgent.Collapse(parameters);

        if (await InitialiseNanoAgent(parameters, agent))
        {
            return agent;
        }

        return null;
    }

    /// <inheritdoc />
    public override async Task Terminate(NanoTerminationParameters parameters)
    {
        await base.Terminate(parameters);
        await TerminateNanoReferences();
        await SplinterEnvironment.TeraAgentContainer.Deregister(this);
        await DeregisterOnTermination();
    }

    /// <inheritdoc />
    protected override async Task InitialiseNanoReferences()
    {
        await base.InitialiseNanoReferences();

        if (HasKnowledge && (_knowledgeReference?.HasNoReference ?? true))
        {
            _knowledgeReference = await CollapseNanoReference(TeraKnowledgeNanoTypeId);
        }
    }

    /// <inheritdoc />
    protected override async Task TerminateNanoReferences()
    {
        await base.TerminateNanoReferences();
        await TerminateNanoReference(_knowledgeReference);
    }

    private async Task<NanoInitialisationParameters> ValidateServiceScope(
        NanoInitialisationParameters parameters)
    {
        if (parameters.ServiceScope == null!)
        {
            parameters = parameters with
            {
                ServiceScope = await SuperpositionAgent.Scope.Start()
            };
        }

        return parameters;
    }

    private async Task InitialiseNanoTable(NanoInitialisationParameters? parameters = null) 
    {
        if (!parameters?.RegisterNanoTable ?? false)
        {
            return;
        }

        if (_nanoTable != null)
        {
            return;
        }

        _nanoTable = await Scope.Resolve<INanoTable>();
    }

    private async Task InternalInitialiseNanoReferences(NanoInitialisationParameters? parameters = null)
    {
        if (!parameters?.RegisterNanoReferences ?? false)
        {
            return;
        }

        await InitialiseNanoReferences();
    }

    private async Task Register(NanoInitialisationParameters parameters)
    {
        if (!parameters.Register)
        {
            return;
        }

        var registerParameters = new TeraAgentRegistrationParameters
        {
            NanoInstanceId = InstanceId,
            TeraAgentStatus = TeraAgentStatus.Running,
            TeraId = parameters.TeraId
        };

        TeraId = await TeraRegistryAgent.Register(registerParameters);

        if (RegisterInContainer)
        {
            await SplinterEnvironment.TeraAgentContainer.Register(this);
        }
    }

    private async Task DeregisterOnTermination()
    {
        var deregisterParameters = new TeraAgentDeregistrationParameters
        {
            TeraId = TeraId
        };

        await TeraRegistryAgent.Deregister(deregisterParameters);
    }
}