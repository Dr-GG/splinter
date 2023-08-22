using System;

namespace Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;

/// <summary>
/// The interface of a service that is responsible for caching the registration and retrieval of Tera Agent ID's.
/// </summary>
public interface ITeraAgentCache
{
    /// <summary>
    /// Registers the ID of a Tera Agent and its associated logical ID.
    /// </summary>
    void RegisterTeraId(Guid teraId, long id);

    /// <summary>
    /// Attempts to find the logical ID associated with a Tera Agent ID.
    /// </summary>
    bool TryGetTeraId(Guid teraId, out long id);
}