using System.Collections.Generic;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;

namespace Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge
{
    public interface ILanguageKnowledgeAgent : ITeraKnowledgeAgent
    {
        IEnumerable<string> SaidPhrases { get; }
    }
}
