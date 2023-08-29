using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Registration;

namespace Splinter.NanoTypes.Database.Interfaces.Services.Registration;

/// <summary>
/// The service responsible for the registration and disposal of Tera Agent instances.
/// </summary>
public interface ITeraAgentRegistrationService
{
    /// <summary>
    /// Registers a single Tera Agent against an ITeraPlatform instance based on specified parameters.
    /// </summary>
    Task<Guid> Register(long teraPlatformId, TeraAgentRegistrationParameters parameters);

    /// <summary>
    /// Deregisters a single Tera Agent based on specified parameters.
    /// </summary>
    Task Deregister(TeraAgentDeregistrationParameters parameters);
}