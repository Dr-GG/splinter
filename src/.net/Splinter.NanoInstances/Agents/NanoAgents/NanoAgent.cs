using System;
using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.NanoWaveFunctions;
using Splinter.NanoInstances.Services.Superposition;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Exceptions.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Interfaces.ServiceScope;
using Splinter.NanoTypes.Interfaces.WaveFunctions;

namespace Splinter.NanoInstances.Agents.NanoAgents;

/// <summary>
/// The default implementation of the INanoAgent interface.
/// </summary>
public abstract class NanoAgent : INanoAgent
{
    private ITeraAgent? _parent;

    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <inheritdoc />
    public abstract SplinterId TypeId { get; }

    /// <inheritdoc />
    public abstract SplinterId InstanceId { get; }

    /// <inheritdoc />
    public virtual bool HasNoTeraParent => _parent == null;

    /// <inheritdoc />
    public virtual bool HasTeraParent => _parent != null;

    /// <inheritdoc />
    public virtual bool HasNoNanoTable => HasNoTeraParent || TeraParent.HasNoNanoTable;

    /// <inheritdoc />
    public virtual bool HasNanoTable => HasTeraParent && TeraParent.HasNanoTable;

    /// <inheritdoc />
    public virtual HolonType HolonType => HolonType.Nano;

    /// <inheritdoc />
    public IServiceScope Scope { get; protected set; } = null!;

    /// <inheritdoc />
    public virtual ITeraAgent TeraParent
    {
        get
        {
            if (_parent == null)
            {
                throw new TeraParentNotInitialisedException();
            }

            return _parent;
        }
        set => _parent = value;
    }

    /// <inheritdoc />
    public virtual INanoTable NanoTable
    {
        get
        {
            if (HasTeraParent && TeraParent.HasNanoTable)
            {
                return TeraParent.NanoTable;
            }

            throw new NanoTableNotInitialisedException();
        }
    }

    /// <inheritdoc />
    public ISuperpositionAgent SuperpositionAgent => SplinterEnvironment.SuperpositionAgent;

    /// <inheritdoc />
    public ITeraPlatformAgent TeraPlatformAgent => SplinterEnvironment.TeraPlatformAgent;

    /// <inheritdoc />
    public ITeraRegistryAgent TeraRegistryAgent => SplinterEnvironment.TeraRegistryAgent;

    /// <inheritdoc />
    public ITeraMessageAgent TeraMessageAgent => SplinterEnvironment.TeraMessageAgent;
    
    /// <summary>
    /// The possible ID of the Tera Parent associated with this agent.
    /// </summary>
    protected Guid? TeraParentTeraId
    {
        get
        {
            if (HolonType == HolonType.Tera)
            {
                return ((ITeraAgent)this).TeraId;
            }
                
            if (HasTeraParent)
            {
                return TeraParent.TeraId;
            }

            return null;
        }
    }

    /// <summary>
    /// The INanoWaveFunction to be used for collapsing purposes of a Nano Type.
    /// </summary>
    protected INanoWaveFunction NanoWaveFunction { get; set; } = new EmptyNanoWaveFunction();

    /// <inheritdoc />
    public virtual async Task Initialise(NanoInitialisationParameters parameters)
    {
        Scope = parameters.ServiceScope;
            
        await InternalInitialiseNanoReferences(parameters);
    }

    /// <inheritdoc />
    public virtual async Task<INanoAgent?> Collapse(NanoCollapseParameters parameters)
    {
        var collapseParameters = ConfigureCollapseParameters(parameters);
        var agent = await NanoWaveFunction.Collapse(parameters);

        if (await InitialiseNanoAgent(parameters, agent))
        {
            return agent;
        }

        if (HasTeraParent)
        {
            agent = await TeraParent.Collapse(collapseParameters);

            if (await InitialiseNanoAgent(collapseParameters, agent))
            {
                return agent;
            }
        }

        if (SplinterEnvironment.Status != SplinterEnvironmentStatus.Initialised)
        {
            return null;
        }

        agent = await SplinterEnvironment.SuperpositionAgent.Collapse(collapseParameters);

        if (await InitialiseNanoAgent(collapseParameters, agent))
        {
            return agent;
        }

        return null;
    }

    /// <inheritdoc />
    public async Task Lock()
    {
        await _lock.WaitAsync();
    }

    /// <inheritdoc />
    public Task Unlock()
    {
        _lock.Release();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Synch(INanoAgent nanoAgent)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task Dispose(NanoDisposeParameters parameters)
    {
        await DisposeNanoTypeDependency();
        await Scope.DisposeAsync();
        await InternalDisposeNanoReferences();
    }

    /// <summary>
    /// Gets a new IServiceScope instance.
    /// </summary>
    protected async Task<IServiceScope> NewScope()
    {
        return await Scope.Start();
    }

    /// <summary>
    /// Collapses a Nano Type ID.
    /// </summary>
    protected async Task<INanoReference> CollapseNanoReference(SplinterId nanoTypeId)
    {
        return await CollapseNanoReference(nanoTypeId.Guid);
    }

    /// <summary>
    /// Collapses a Nano Type ID.
    /// </summary>
    protected async Task<INanoReference> CollapseNanoReference(Guid nanoTypeId)
    {
        var parameters = new NanoCollapseParameters
        {
            NanoTypeId = nanoTypeId
        };

        return await CollapseNanoReference(parameters);
    }

    /// <summary>
    /// Collapses a Nano Type ID based on specified collapse parameters.
    /// </summary>
    protected async Task<INanoReference> CollapseNanoReference(NanoCollapseParameters parameters)
    {
        var collapseParameters = parameters with
        {
            TeraAgentId = TeraParentTeraId,
            Initialise = false
        };
        var initialiseParameters = parameters with
        {
            TeraAgentId = TeraParentTeraId
        };
        var nanoAgent = await Collapse(collapseParameters);
        var nanoReference = new NanoReference();

        await nanoReference.Initialise(nanoAgent);
        await InitialiseNanoReference(initialiseParameters, nanoReference);

        return nanoReference;
    }

    /// <summary>
    /// Initialises an INanoAgent instance that was collapsed.
    /// </summary>
    protected virtual async Task<bool> InitialiseNanoAgent(
        NanoCollapseParameters parameters,
        INanoAgent? nanoAgent)
    {
        if (nanoAgent == null)
        {
            return false;
        }

        if (nanoAgent.HolonType == HolonType.Nano)
        {
            nanoAgent.TeraParent = TeraParent;
        }

        if (!parameters.Initialise)
        {
            return true;
        }

        var initParameters = parameters.Initialisation
                             ?? await GetDefaultNanoReferenceInitialisationParameters();

        await nanoAgent.Initialise(initParameters);

        return true;
    }

    /// <summary>
    /// Initialises all Nano Type references of the agent.
    /// </summary>
    protected virtual Task InitialiseNanoReferences()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initialises the Nano Agent contained within an INanoReference instance.
    /// </summary>
    protected virtual async Task InitialiseNanoReference(
        NanoCollapseParameters parameters,
        INanoReference reference)
    {
        if (reference.HasNoReference)
        {
            return;
        }

        if (HasNanoTable)
        {
            await NanoTable.Register(reference);
        }

        await InitialiseNanoAgent(parameters, reference.Reference);
    }

    /// <summary>
    /// Disposes of all INanoReferences of the agent.
    /// </summary>
    /// <returns></returns>
    protected virtual Task DisposeNanoReferences()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes of an INanoReference instance.
    /// </summary>
    protected virtual async Task DisposeNanoReference(INanoReference? reference)
    {
        if (reference == null)
        {
            return;
        }

        if (HasNanoTable)
        {
            await NanoTable.Dispose(reference);
        }

        if (reference.HasNoReference)
        {
            return;
        }

        var disposeParameters = new NanoDisposeParameters();

        await reference.Reference.Dispose(disposeParameters);
        await reference.Initialise(null);
    }

    private async Task InternalInitialiseNanoReferences(NanoInitialisationParameters parameters)
    {
        if (HolonType != HolonType.Nano)
        {
            return;
        }

        if (!parameters.RegisterNanoReferences)
        {
            return;
        }

        await InitialiseNanoReferences();
    }

    private async Task InternalDisposeNanoReferences()
    {
        if (HolonType != HolonType.Nano)
        {
            return;
        }

        await DisposeNanoReferences();
    }

    private NanoCollapseParameters ConfigureCollapseParameters(NanoCollapseParameters parameters)
    {
        var newParameters = parameters;

        if (HasTeraParent)
        {
            newParameters = newParameters with {TeraAgentId = TeraParent.TeraId};
        }

        return newParameters;
    }

    private async Task<NanoInitialisationParameters> GetDefaultNanoReferenceInitialisationParameters()
    {
        var result = new NanoInitialisationParameters
        {
            ServiceScope = await NewScope(),
            TeraId = TeraParentTeraId
        };

        return result;
    }

    private async Task DisposeNanoTypeDependency()
    {
        if (HolonType != HolonType.Nano
            || HasNoTeraParent)
        {
            return;
        }

        var disposeParameters = new NanoTypeDependencyDisposeParameters
        {
            TeraId = TeraParent.TeraId,
            NanoType = TypeId
        };

        await SuperpositionAgent.Sync(disposeParameters);
    }
}