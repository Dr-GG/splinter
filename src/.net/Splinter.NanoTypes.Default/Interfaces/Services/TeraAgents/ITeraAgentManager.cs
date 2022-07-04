using System;
using System.Threading.Tasks;

namespace Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;

public interface ITeraAgentManager
{
    Task<long?> GetTeraId(Guid teraId);
    Task RegisterTeraId(Guid teraId, long id);
}