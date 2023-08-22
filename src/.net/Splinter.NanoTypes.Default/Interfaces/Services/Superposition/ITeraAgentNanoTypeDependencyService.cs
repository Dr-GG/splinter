using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Superposition;

/// <summary>
/// The interface of the service responsible for registering Nano Type dependencies of a Tera Agent.
/// </summary>
public interface ITeraAgentNanoTypeDependencyService
{
    /// <summary>
    /// Increments the number of dependencies between a Tera Agent and a Nano Type ID with 1.
    /// </summary>
    Task IncrementTeraAgentNanoTypeDependencies(Guid teraId, SplinterId nanoType, int numberOfDependencies = 1);

    /// <summary>
    /// Decrements the number of dependencies between a Tera Agent and a Nano Type ID with 1.
    /// </summary>
    Task DecrementTeraAgentNanoTypeDependencies(Guid teraId, SplinterId nanoType, int numberOfDependencies = 1);

    /// <summary>
    /// Disposes and removes all Nano Type dependencies of a Tera Agent.
    /// </summary>
    Task DisposeTeraAgentNanoTypeDependencies(Guid teraId);

    /// <summary>
    /// Signals that a Nano Type dependencies of a Tera Agent has been recollapsed.
    /// </summary>
    Task<Guid?> SignalNanoTypeRecollapses(Guid sourceTeraId, Guid nanoTypeId, IEnumerable<Guid>? teraIds);
}