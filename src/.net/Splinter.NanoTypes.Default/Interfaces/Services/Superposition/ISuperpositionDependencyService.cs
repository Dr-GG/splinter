using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Superposition;

/// <summary>
/// The interface of the service responsible for registering Nano Type dependencies.
/// </summary>
public interface ISuperpositionDependencyService
{
    /// <summary>
    /// Registers a dependency between a Nano Type and Nano Instance ID.
    /// </summary>
    Task Register(SplinterId nanoTypeId, SplinterId nanoInstanceId);

    /// <summary>
    /// Registers a dependency between a Tera Agent and a Nano Type ID.
    /// </summary>
    Task Register(Guid teraAgentId, Guid nanoTypeId);
}