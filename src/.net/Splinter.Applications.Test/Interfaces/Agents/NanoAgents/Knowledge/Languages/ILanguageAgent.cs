using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge.Languages;

public interface ILanguageAgent : INanoAgent
{
    Task<string> SayHello();
    Task<string> SayTest();
    Task<string> SayGoodbye();
}