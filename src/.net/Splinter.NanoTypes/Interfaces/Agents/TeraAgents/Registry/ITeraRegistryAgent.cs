using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Registration;

namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;

/// <summary>
/// The ITeraAgent that enables ITeraAgent instances to register and dispose themselves via a global registry.
/// </summary>
public interface ITeraRegistryAgent : ITeraAgent
{
    /// <summary>
    /// Registers an ITeraAgent instance based on specified registration parameters.
    /// </summary>
    Task<Guid> Register(TeraAgentRegistrationParameters parameters);

    /// <summary>
    /// Disposes an ITeraAgent instance based on specified disposal parameters.
    /// </summary>
    Task Dispose(TeraAgentDisposeParameters parameters);
}