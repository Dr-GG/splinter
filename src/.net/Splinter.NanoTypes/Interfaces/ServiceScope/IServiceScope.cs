using System;
using System.Threading.Tasks;

namespace Splinter.NanoTypes.Interfaces.ServiceScope;

/// <summary>
/// The interface that mimics a service capable of providing dependency injection.
/// </summary>
public interface IServiceScope : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Starts the current scope block of the service.
    /// </summary>
    IServiceScope StartSync();

    /// <summary>
    /// Starts the current scope block of the service.
    /// </summary>
    Task<IServiceScope> Start();

    /// <summary>
    /// Resolves an interface to an implementation.
    /// </summary>
    TService ResolveSync<TService>() where TService : class;

    /// <summary>
    /// Resolves an interface to an implementation.
    /// </summary>
    Task<TService> Resolve<TService>() where TService : class;
}