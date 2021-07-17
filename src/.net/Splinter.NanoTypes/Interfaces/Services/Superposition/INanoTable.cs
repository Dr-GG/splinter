using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Splinter.NanoTypes.Interfaces.Services.Superposition
{
    public interface INanoTable
    {
        Task Register(INanoReference reference);
        Task<IEnumerable<INanoReference>> Fetch(Guid nanoTypeId);
        Task Dispose(INanoReference reference);
    }
}