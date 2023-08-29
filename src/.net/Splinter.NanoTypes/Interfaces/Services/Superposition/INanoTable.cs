using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Splinter.NanoTypes.Interfaces.Services.Superposition;

/// <summary>
/// The interface that acts as a repository of INanoReference instances.
/// </summary>
public interface INanoTable
{
    /// <summary>
    /// Registers a single INanoReference instance.
    /// </summary>
    Task Register(INanoReference reference);

    /// <summary>
    /// Fetches all INanoReference instances that implements a specific Nano Type.
    /// </summary>
    Task<IEnumerable<INanoReference>> Fetch(Guid nanoTypeId);

    /// <summary>
    /// Removes a single INanoReference instance.
    /// </summary>
    Task Deregister(INanoReference reference);
}