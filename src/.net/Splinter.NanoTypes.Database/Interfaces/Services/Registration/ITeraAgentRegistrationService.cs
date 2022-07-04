using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Registration;

namespace Splinter.NanoTypes.Database.Interfaces.Services.Registration;

public interface ITeraAgentRegistrationService
{
    Task<Guid> Register(long teraPlatformId, TeraAgentRegistrationParameters parameters);
    Task Dispose(TeraAgentDisposeParameters parameters);
}