using System;
using System.Threading.Tasks;

namespace Splinter.NanoTypes.Interfaces.ServiceScope;

public interface IServiceScope : IAsyncDisposable, IDisposable
{
    IServiceScope StartSync();
    Task<IServiceScope> Start();
    TService ResolveSync<TService>() where TService : class;
    Task<TService> Resolve<TService>() where TService : class;
}