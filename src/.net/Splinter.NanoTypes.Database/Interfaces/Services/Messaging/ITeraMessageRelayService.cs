using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoTypes.Database.Interfaces.Services.Messaging;

/// <summary>
/// The service responsible for relaying TeraMessages between Tera Agents.
/// </summary>
public interface ITeraMessageRelayService
{
    /// <summary>
    /// Relays a message between Tera Agent instances based on a collection of parameters.
    /// </summary>
    Task<TeraMessageResponse> Relay(TeraMessageRelayParameters parameters);
}