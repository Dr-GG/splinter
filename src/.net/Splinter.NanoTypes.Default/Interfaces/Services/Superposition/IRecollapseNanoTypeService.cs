using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Superposition;

/// <summary>
/// The interface of the service responsible for the recollapse of previously collapsed Nano Types.
/// </summary>
public interface IRecollapseNanoTypeService
{
    /// <summary>
    /// Starts the process of recollapsing a Nano Type.
    /// </summary>
    /// <returns>
    /// The number of recollapsed agents.
    /// </returns>
    Task<int> Recollapse(INanoAgent parent, TeraMessage recollapseMessage);
}