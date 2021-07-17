using System.Threading.Tasks;
using Autofac;
using Splinter.NanoTypes.Domain.Exceptions.ServiceScope;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoInstances.Services.ServiceScope
{
    public class AutofacServiceScope : IServiceScope
    {
        private readonly ILifetimeScope _scope;

        public AutofacServiceScope(ILifetimeScope scope)
        {
            _scope = scope.BeginLifetimeScope();
        }

        public IServiceScope StartSync()
        {
            return new AutofacServiceScope(_scope);
        }

        public Task<IServiceScope> Start()
        {
            return Task.FromResult<IServiceScope>(
                new AutofacServiceScope(_scope));
        }

        public TService ResolveSync<TService>() where TService : class
        {
            var service = _scope.Resolve<TService>();

            if (service == null)
            {
                throw new ServiceUnresolvedException(typeof(TService));
            }

            return service;
        }

        public Task<TService> Resolve<TService>() where TService : class
        {
            return Task.FromResult(ResolveSync<TService>());
        }

        public async ValueTask DisposeAsync()
        {
            await _scope.DisposeAsync();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
