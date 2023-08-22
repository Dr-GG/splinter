using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;

/// <summary>
/// The ITeraAgent interface responsible for messaging functionality.
/// </summary>
public interface ITeraMessageAgent : ITeraAgent
{
    /// <summary>
    /// Relays or sends messages.
    /// </summary>
    Task<TeraMessageResponse> Send(TeraMessageRelayParameters parameters);

    /// <summary>
    /// Retrieves messages of a specified ITeraAgent instance.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    Task<IEnumerable<TeraMessage>> Dequeue(TeraMessageDequeueParameters parameters);

    /// <summary>
    /// Synchronises the state of existing messages.
    /// </summary>
    Task Sync(TeraMessageSyncParameters parameters);
}