using System.Threading.Tasks;
using Autofac;
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
        var service = _scope.Resolve<TService>();

        return service ?? throw new ServiceUnresolvedException(typeof(TService));
    }

    /// <inheritdoc />
    public Task<TService> Resolve<TService>() where TService : class
    {
        return Task.FromResult(ResolveSync<TService>());
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _scope.Dispose();
    }
}