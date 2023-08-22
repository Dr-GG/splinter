using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Registration;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;

namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

/// <summary>
/// The extended INanoAgent that reflects a Tera Type or Tera Agent.
/// </summary>
public interface ITeraAgent : INanoAgent
{
    /// <summary>
    /// The unique ID of the ITeraAgent.
    /// </summary>
    public Guid TeraId { get; }

    /// <summary>
    /// The ITeraKnowledgeAgent that embodies the knowledge and intelligence of the ITeraAgent.
    /// </summary>
    public ITeraKnowledgeAgent Knowledge { get; }

    /// <summary>
    /// Registers the ITeraAgent with the global ITeraAgentRegistry instance.
    /// </summary>
    Task RegisterSelf(TeraAgentRegistrationParameters parameters);

    /// <summary>
    /// Executes the intelligence and knowledge of the ITeraAgent.
    /// </summary>
    Task Execute(TeraAgentExecutionParameters parameters);
}