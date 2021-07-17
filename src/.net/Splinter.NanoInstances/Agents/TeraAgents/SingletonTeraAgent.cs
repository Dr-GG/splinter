using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Environment;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;

namespace Splinter.NanoInstances.Agents.TeraAgents
{
    public abstract class SingletonTeraAgent : TeraAgent
    {
        private readonly SemaphoreSlim _lock = new(1, 1);

        private bool _disposed;
        private bool _initialised;

        public override async Task Initialise(NanoInitialisationParameters parameters)
        {
            if (_initialised)
            {
                return;
            }

            try
            {
                await _lock.WaitAsync();

                if (_initialised)
                {
                    return;
                }

                await SingletonInitialise(parameters);
            }
            finally
            {
                _lock.Release();
            }
        }

        public override async Task Dispose(NanoDisposeParameters parameters)
        {
            if (parameters.Force || SplinterEnvironment.Status == SplinterEnvironmentStatus.Disposing)
            {
                await InternalDispose(parameters);
            }
        }

        protected virtual async Task SingletonInitialise(NanoInitialisationParameters parameters)
        {
            await base.Initialise(parameters);

            _initialised = true;
        }

        protected virtual async Task SingletonDispose(NanoDisposeParameters parameters)
        {
            await base.Dispose(parameters);

            _disposed = true;
        }

        private async Task InternalDispose(NanoDisposeParameters parameters)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                await _lock.WaitAsync();

                if (_disposed)
                {
                    return;
                }

                await SingletonDispose(parameters);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
