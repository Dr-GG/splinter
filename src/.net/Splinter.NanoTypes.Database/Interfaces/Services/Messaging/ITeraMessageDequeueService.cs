using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoTypes.Database.Interfaces.Services.Messaging;

/// <summary>
/// The service responsible for retrieving a collection of TeraMessage instances.
/// </summary>
public interface ITeraMessageDequeueService
{
    /// <summary>
    /// Retrieves a collection of TeraMessages based on specified parameters.
    /// </summary>
    Task<IEnumerable<TeraMessage>> Dequeue(TeraMessageDequeueParameters parameters);
}