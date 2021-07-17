using System.Threading.Tasks;
using Autofac;

namespace Splinter.Common.Services
{
    public class AutofacServiceScope : IServiceScope
    {
        private readonly ILifetimeScope _scope;

        public AutofacServiceScope(ILifetimeScope scope)
        {
            _scope = scope.BeginLifetimeScope();
        }

        public Task<IServiceScope> Start()
        {
            return Task.FromResult<IServiceScope>(
                new AutofacServiceScope(_scope));
        }

        public Task<TService> Resolve<TService>() where TService : class
        {
            var service = _scope.Resolve<TService>();

            if (service == null)
            {
                throw new ServiceUnresolvedException(typeof(TService));
            }

            return Task.FromResult(service);
        }

        public async ValueTask DisposeAsync()
        {
            await _scope.DisposeAsync();
        }
    }
}
