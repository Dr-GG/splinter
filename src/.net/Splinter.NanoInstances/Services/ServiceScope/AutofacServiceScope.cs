using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Splinter.NanoTypes.Domain.Exceptions.ServiceScope;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoInstances.Services.ServiceScope;

/// <summary>
/// The default implementation of the IServiceScope using Autofac.
/// </summary>
public class AutofacServiceScope : IServiceScope
{
    private readonly ILifetimeScope _scope;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public AutofacServiceScope(ILifetimeScope scope)
    {
        _scope = scope.BeginLifetimeScope();
    }

    /// <inheritdoc />
    public IServiceScope StartSync()
    {
        return new AutofacServiceScope(_scope);
    }

    /// <inheritdoc />
    public Task<IServiceScope> Start()
    {
        return Task.FromResult<IServiceScope>(
            new AutofacServiceScope(_scope));
    }

    /// <inheritdoc />
    public TService ResolveSync<TService>() where TService : class
    {
        try
        {
            return _scope.Resolve<TService>();
        }
        catch (DependencyResolutionException error)
        {
            throw new ServiceUnresolvedException(typeof(TService), error);
        }
    }

    /// <inheritdoc />
    public Task<TService> Resolve<TService>() where TService : class
    {
        return Task.FromResult(ResolveSync<TService>());
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        Dispose(true);

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _scope.Dispose();
    }
}