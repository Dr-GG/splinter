using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;

namespace Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;

public interface ITeraKnowledgeAgent : INanoAgent
{
    Task Execute(TeraAgentExecutionParameters parameters);
}