using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Default.Domain.Messaging.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;

namespace Splinter.NanoInstances.Default.Agents.NanoAgents.Knowledge
{
    public class TeraKnowledgeAgent : NanoAgent, ITeraKnowledgeAgent
    {
        public static readonly SplinterId NanoTypeId = SplinterIdConstants.TeraDefaultKnowledgeNanoTypeId;
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Default Tera Knowledge Agent",
            Version = "1.0.0",
            Guid = new Guid("{39E1E8C9-4D9D-4F15-99C6-C76754E3D7E9}")
        };

        private ITeraMessageQueue? _messageQueue;

        protected bool HasNoMessageQueue => _messageQueue == null;
        protected ITeraMessageQueue MessageQueue
        {
            get
            {
                if (_messageQueue == null)
                {
                    throw new NanoServiceNotInitialisedException(typeof(ITeraMessageQueue));
                }

                return _messageQueue;
            }
        }

        public override SplinterId TypeId => SplinterIdConstants.TeraDefaultKnowledgeNanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;

        public override async Task Initialise(NanoInitialisationParameters parameters)
        {
            await base.Initialise(parameters);
            await InitialiseTeraMessageQueue();
        }

        public virtual async Task Execute(TeraAgentExecutionParameters parameters)
        {
            await ProcessTeraMessages();
        }

        protected virtual async Task<bool> ProcessTeraMessage(TeraMessage teraMessage)
        {
            var handled = true;

            switch (teraMessage.Code)
            {
                case TeraMessageCodeConstants.Recollapse:
                    await Recollapse(teraMessage);
                    break;

                default:
                    handled = false;
                    break;
            }

            return handled;
        }

        private async Task Recollapse(TeraMessage teraMessage)
        {
            await using var scope = await NewScope();
            var service = await scope.Resolve<IRecollapseNanoTypeService>();
            var recollapseData = teraMessage.JsonMessage<RecollapseTeraMessageData>();
            var wasSuccessful = true;

            try
            {
                await service.Recollapse(this, teraMessage);
            }
            catch
            {
                wasSuccessful = false;
            }

            if (recollapseData == null)
            {
                return;
            }

            var recollapseOperationParameters = new NanoRecollapseOperationParameters
            {
                TeraId = TeraParent.TeraId,
                IsSuccessful = wasSuccessful,
                NanoRecollapseOperationId = recollapseData.NanoTypeRecollapseOperationId
            };

            await SuperpositionAgent.Sync(recollapseOperationParameters);
        }

        private async Task ProcessTeraMessages()
        {
            if (HasNoMessageQueue)
            {
                return;
            }

            var syncs = new List<TeraMessageSyncParameter>();

            while (await MessageQueue.HasNext())
            {
                var message = await MessageQueue.Next();
                var sync = await InternalProcessMessage(message);

                syncs.Add(sync);
            }

            var syncParameters = new TeraMessageSyncParameters
            {
                Syncs = syncs
            };

            await TeraMessageAgent.Sync(syncParameters);
        }

        private async Task<TeraMessageSyncParameter> InternalProcessMessage(TeraMessage teraMessage)
        {
            var result = new TeraMessageSyncParameter
            {
                TeraMessageId = teraMessage.Id
            };

            try
            {
                await ProcessTeraMessage(teraMessage);
            }
            catch (Exception error)
            {
                result = result with
                {
                    ErrorCode = TeraMessageErrorCode.Unknown,
                    ErrorMessage = error.ToString(),
                    ErrorStackTrace = error.StackTrace
                };
            }
            finally
            {
                result = result with
                {
                    CompletionTimestamp = DateTime.UtcNow
                };
            }

            return result;
        }

        private async Task InitialiseTeraMessageQueue()
        {
            if (HasNoTeraParent)
            {
                return;
            }

            _messageQueue = await Scope.Resolve<ITeraMessageQueue>();

            await _messageQueue.Initialise(TeraParent);
        }
    }
}
