using System.Collections.Generic;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;

namespace Splinter.NanoInstances.Default.Tests.Agents.Knowledge;

public interface ILanguageKnowledgeAgent : ITeraKnowledgeAgent
{
    IEnumerable<string> SaidPhrases { get; }
}