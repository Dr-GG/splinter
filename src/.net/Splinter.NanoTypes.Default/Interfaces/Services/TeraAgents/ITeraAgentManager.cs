using System;
using System.Threading.Tasks;

namespace Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;

/// <summary>
/// The interface of the service responsible for managing the registration and retrieval of Tera Agent ID's.
/// </summary>
public interface ITeraAgentManager
{
    /// <summary>
    /// Gets the logical ID associated with a Tera Agent ID.
    /// </summary>
    Task<long?> GetTeraId(Guid teraId);

    /// <summary>
    /// Registers the ID of a Tera Agent and its associated logical ID.
    /// </summary>
    Task RegisterTeraId(Guid teraId, long id);
}