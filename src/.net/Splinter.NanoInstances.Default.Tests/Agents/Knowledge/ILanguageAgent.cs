using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Default.Tests.Agents.Knowledge;

public interface ILanguageAgent : INanoAgent
{
    Task<string> SayHello();
    Task<string> SayTest();
    Task<string> SayGoodbye();
}