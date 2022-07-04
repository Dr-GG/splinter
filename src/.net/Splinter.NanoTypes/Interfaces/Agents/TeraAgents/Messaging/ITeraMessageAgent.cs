using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;

public interface ITeraMessageAgent : ITeraAgent
{
    Task<TeraMessageResponse> Send(TeraMessageRelayParameters parameters);
    Task<IEnumerable<TeraMessage>> Dequeue(TeraMessageDequeueParameters parameters);
    Task Sync(TeraMessageSyncParameters parameters);
}