using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Messaging;

/// <summary>
/// The interface of the service responsible for retrieving batches of TeraMessage instances.
/// </summary>
public interface ITeraMessageQueue
{
    /// <summary>
    /// Initialises the ITeraMessageQueue according to an ITeraAgent instance.
    /// </summary>
    Task Initialise(ITeraAgent teraAgent);

    /// <summary>
    /// Gets the value indicating if there are one or messages yet to be processed.
    /// </summary>
    Task<bool> HasNext();

    /// <summary>
    /// Gets the next TeraMessage.
    /// </summary>
    Task<TeraMessage> Next();

    /// <summary>
    /// Terminates one or more TeraMessage instances.
    /// </summary>
    Task Terminate(TeraMessageSyncParameters parameters);
}