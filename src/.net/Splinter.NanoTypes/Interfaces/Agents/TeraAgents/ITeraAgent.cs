using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Registration;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;

namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents
{
    public interface ITeraAgent : INanoAgent
    {
        public Guid TeraId { get; }
        public ITeraKnowledgeAgent Knowledge { get; }

        Task RegisterSelf(TeraAgentRegistrationParameters parameters);
        Task Execute(TeraAgentExecutionParameters parameters);
    }
}
