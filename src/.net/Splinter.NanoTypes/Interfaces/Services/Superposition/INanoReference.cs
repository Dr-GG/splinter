using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoTypes.Interfaces.Services.Superposition;

/// <summary>
/// The interfaces that holds a reference to a Nano Type instance.
/// </summary>
public interface INanoReference
{
    /// <summary>
    /// Gets the value indicating if a current reference does not exist.
    /// </summary>
    bool HasNoReference { get; }

    /// <summary>
    /// Gets the value indicating if a current reference exists.
    /// </summary>
    bool HasReference { get; }

    /// <summary>
    /// The INanoAgent reference.
    /// </summary>
    INanoAgent Reference { get; }
        
    /// <summary>
    /// Initialises the reference based on a INanoAgent instance.
    /// </summary>
    Task Initialise(INanoAgent? reference);
}