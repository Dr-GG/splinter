using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Default.Services.Superposition
{
    public class SuperpositionSingletonRegistry : ISuperpositionSingletonRegistry
    {
        private readonly ReaderWriterLock _rwLock = new();
        private readonly IDictionary<Guid, INanoAgent> _singletonMap = new Dictionary<Guid, INanoAgent>();
        private readonly SuperpositionSettings _settings;

        public SuperpositionSingletonRegistry(SuperpositionSettings settings)
        {
            _settings = settings;
        }

        public async Task<INanoAgent> Register(Guid nanoTypeId, INanoAgent singleton)
        {
            try
            {
                _rwLock.AcquireWriterLock(_settings.RegistryTimeoutSpan);

                if (_singletonMap.TryGetValue(nanoTypeId, out var reference))
                {
                    if (reference.InstanceId.Guid == singleton.InstanceId.Guid)
                    {
                        return reference;
                    }

                    await DisposeSingletonReference(reference);
                }

                _singletonMap[nanoTypeId] = singleton;

                return singleton;
            }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }
        }

        public Task<INanoAgent?> Fetch(Guid nanoTypeId)
        {
            try
            {
                _rwLock.AcquireReaderLock(_settings.RegistryTimeoutSpan);

                return _singletonMap.TryGetValue(nanoTypeId, out var result) 
                    ? Task.FromResult<INanoAgent?>(result) 
                    : Task.FromResult<INanoAgent?>(null);
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        private static async Task DisposeSingletonReference(INanoAgent nanoAgent)
        {
            var parameters = new NanoDisposeParameters
            {
                Force = true
            };

            await nanoAgent.Dispose(parameters);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                _rwLock.AcquireWriterLock(_settings.RegistryTimeoutSpan);

                foreach (var singleton in _singletonMap)
                {
                    await DisposeSingletonReference(singleton.Value);
                }

                _singletonMap.Clear();
            }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }
        }
    }
}
