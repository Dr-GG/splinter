using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Exceptions.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Registration;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Agents.TeraAgents
{
    public abstract class TeraAgent : NanoAgent, ITeraAgent
    {
        private INanoTable? _nanoTable;
        private INanoReference? _knowledgeReference;

        public override bool HasNoTeraParent => true;
        public override bool HasTeraParent => false;
        public override bool HasNoNanoTable => _nanoTable == null;
        public override bool HasNanoTable => _nanoTable != null;
        public override HolonType HolonType => HolonType.Tera;
        public override ITeraAgent TeraParent => this;
        public Guid TeraId { get; protected set; }
        public override INanoTable NanoTable
        {
            get
            {
                if (_nanoTable == null)
                {
                    throw new NanoTableNotInitialisedException();
                }

                return _nanoTable;
            }
        }

        public ITeraKnowledgeAgent Knowledge
        {
            get
            {
                if (_knowledgeReference.IsNullOrEmpty())
                {
                    throw new TeraKnowledgeNotInitialisedException();
                }

                return _knowledgeReference.Typed<ITeraKnowledgeAgent>();
            }
        }

        protected virtual bool RegisterInContainer => true;
        protected virtual bool HasKnowledge => true;
        protected virtual SplinterId TeraKnowledgeNanoTypeId => SplinterIdConstants.TeraDefaultKnowledgeNanoTypeId;

        public override async Task Initialise(NanoInitialisationParameters parameters)
        {
            var validParameters = await ValidateServiceScope(parameters);

            await base.Initialise(validParameters);

            await Register(validParameters);
            await InitialiseNanoTable(validParameters);
            await InternalInitialiseNanoReferences(validParameters);
        }

        public async Task RegisterSelf(TeraAgentRegistrationParameters parameters)
        {
            var correctParameters = parameters with {NanoInstanceId = InstanceId};

            await InitialiseNanoTable();
            await InternalInitialiseNanoReferences();

            TeraId = await TeraRegistryAgent.Register(correctParameters);
        }

        public virtual async Task Execute(TeraAgentExecutionParameters parameters)
        {
            if (_knowledgeReference == null || _knowledgeReference.HasNoReference)
            {
                return;
            }

            await _knowledgeReference.Typed<ITeraKnowledgeAgent>().Execute(parameters);
        }

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

        public override async Task Dispose(NanoDisposeParameters parameters)
        {
            await base.Dispose(parameters);
            await DisposeNanoReferences();
            await SplinterEnvironment.TeraAgentContainer.Dispose(this);
            await DisposeOnRegistry();
        }

        protected override async Task InitialiseNanoReferences()
        {
            await base.InitialiseNanoReferences();

            if (HasKnowledge)
            {
                _knowledgeReference = await CollapseNanoReference(TeraKnowledgeNanoTypeId);
            }
        }

        protected override async Task DisposeNanoReferences()
        {
            await base.DisposeNanoReferences();
            await DisposeNanoReference(_knowledgeReference);
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

        private async Task DisposeOnRegistry()
        {
            var disposeParameters = new TeraAgentDisposeParameters
            {
                TeraId = TeraId
            };

            await TeraRegistryAgent.Dispose(disposeParameters);
        }
    }
}
