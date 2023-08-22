using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoTypes.Database.Interfaces.Services.Messaging;

/// <summary>
/// The interface that synchronises the state of a TeraMessage instance.
/// </summary>
public interface ITeraMessageSyncService
{
    /// <summary>
    /// Synchronises the state of a TeraMessage instance based on specified parameters.
    /// </summary>
    Task Sync(TeraMessageSyncParameters parameters);
}