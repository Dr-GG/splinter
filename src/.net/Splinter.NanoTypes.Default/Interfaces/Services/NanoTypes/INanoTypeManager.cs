using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;

/// <summary>
/// The interface of a service that is responsible for the registration and retrieval of Nano Type and Nano Instance id's.
/// </summary>
public interface INanoTypeManager
{
    /// <summary>
    /// Registers the Nano Type of an INanoAgent instance.
    /// </summary>
    Task RegisterNanoType(INanoAgent agent);

    /// <summary>
    /// Attempts to fetch the logical ID associated with a Nano Type id.
    /// </summary>
    Task<long?> GetNanoTypeId(SplinterId nanoTypeId);

    /// <summary>
    /// Attempts to fetch the logical ID associated with a Nano Instance id.
    /// </summary>
    Task<long?> GetNanoInstanceId(SplinterId nanoInstanceId);
}