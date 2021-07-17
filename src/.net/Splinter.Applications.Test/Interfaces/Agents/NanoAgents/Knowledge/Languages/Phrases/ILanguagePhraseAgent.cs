using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents;

namespace Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge.Languages.Phrases
{
    public interface ILanguagePhraseAgent : INanoAgent
    {
        Task<string> Speak();
    }
}
