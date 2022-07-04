using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Default.Tests.Agents.Knowledge;

public interface ILanguagePhraseAgent : INanoAgent
{
    Task<string> Speak();
}