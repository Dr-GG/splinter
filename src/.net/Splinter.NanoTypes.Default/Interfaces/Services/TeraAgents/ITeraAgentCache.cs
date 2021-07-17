using System;

namespace Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents
{
    public interface ITeraAgentCache
    {
        void RegisterTeraId(Guid teraId, long id);
        bool TryGetTeraId(Guid teraId, out long id);
    }
}
