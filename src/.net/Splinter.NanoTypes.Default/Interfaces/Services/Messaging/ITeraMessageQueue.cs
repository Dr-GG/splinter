using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Messaging;

public interface ITeraMessageQueue
{
    Task Initialise(ITeraAgent teraAgent);
    Task<bool> HasNext();
    Task<TeraMessage> Next();
    Task Dispose(TeraMessageSyncParameters parameters);
}