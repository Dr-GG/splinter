using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;

namespace Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;

/// <summary>
/// The INanoAgent interface that represents the knowledge of an Tera Agent.
/// </summary>
public interface ITeraKnowledgeAgent : INanoAgent
{
    /// <summary>
    /// Executes the knowledge.
    /// </summary>
    Task Execute(TeraAgentExecutionParameters parameters);
}