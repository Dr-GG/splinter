using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Registration;

namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry
{
    public interface ITeraRegistryAgent : ITeraAgent
    {
        Task<Guid> Register(TeraAgentRegistrationParameters parameters);
        Task Dispose(TeraAgentDisposeParameters parameters);
    }
}
