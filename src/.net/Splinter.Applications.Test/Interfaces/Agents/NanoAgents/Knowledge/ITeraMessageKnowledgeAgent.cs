using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;

namespace Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge;

public interface ITeraMessageKnowledgeAgent : ITeraKnowledgeAgent
{
    long SupportedProcessCount { get; }
    long NotSupportedProcessCount { get; }
}