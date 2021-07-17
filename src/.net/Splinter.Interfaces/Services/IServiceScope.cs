using System;
using System.Threading.Tasks;

namespace Splinter.Interfaces.Services
{
    public interface IServiceScope : IAsyncDisposable
    {
        Task<IServiceScope> Start();
        Task<TService> Resolve<TService>() where TService : class;
    }
}
