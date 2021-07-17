using System;
using System.Threading.Tasks;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.NanoInstances.Default.Agents.NanoAgents.Knowledge;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Messaging;

namespace Splinter.Applications.Test.Agents.NanoAgents.Knowledge
{
    public class TeraMessageKnowledgeAgent : TeraKnowledgeAgent, ITeraMessageKnowledgeAgent
    {
        public new static readonly SplinterId NanoTypeId = TeraMessageSplinterIds.KnowledgeNanoTypeId;
        public new static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Tera Message Knowledge Agent",
            Version = "1.0.0",
            Guid = new Guid("{C5CEB051-61FA-4131-A1A8-8B35B6C9DD25}")
        };

        public long SupportedProcessCount { get; private set; }
        public long NotSupportedProcessCount { get; private set; }

        protected override async Task<bool> ProcessTeraMessage(TeraMessage teraMessage)
        {
            switch (teraMessage.Code)
            {
                case TeraMessageCodeConstants.SupportedTeraMessageCode1:
                case TeraMessageCodeConstants.SupportedTeraMessageCode2:
                case TeraMessageCodeConstants.SupportedTeraMessageCode3:
                    SupportedProcessCount++;

                    return true;

                default:
                    NotSupportedProcessCount++;
                    return await base.ProcessTeraMessage(teraMessage);
            }
        }
    }
}
