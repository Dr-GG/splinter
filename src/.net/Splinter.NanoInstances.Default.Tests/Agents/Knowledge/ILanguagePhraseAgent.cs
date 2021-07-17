using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents;

namespace Splinter.NanoInstances.Default.Tests.Agents.Knowledge
{
    public interface ILanguagePhraseAgent : INanoAgent
    {
        Task<string> Speak();
    }
}
