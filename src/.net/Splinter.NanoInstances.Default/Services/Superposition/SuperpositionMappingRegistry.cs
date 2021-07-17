using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Models;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;

namespace Splinter.NanoInstances.Default.Services.Superposition
{
    public class SuperpositionMappingRegistry : ISuperpositionMappingRegistry
    {
        private readonly ReaderWriterLock _rwLock = new();
        private readonly IDictionary<Guid, InternalSuperpositionMapping> _mappings = new Dictionary<Guid, InternalSuperpositionMapping>();
        private readonly SuperpositionSettings _settings;

        public SuperpositionMappingRegistry(SuperpositionSettings settings)
        {
            _settings = settings;
        }

        public Task Register(params InternalSuperpositionMapping[] mappings)
        {
            if (mappings.IsEmpty())
            {
                return Task.CompletedTask;
            }

            try
            {
                _rwLock.AcquireWriterLock(_settings.RegistryTimeoutSpan);

                foreach (var mapping in mappings)
                {
                    _mappings[mapping.NanoTypeId] = mapping;
                }
            }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }

            return Task.CompletedTask;
        }

        public Task<InternalSuperpositionMapping?> Fetch(Guid nanoTypeId)
        {
            try
            {
                _rwLock.AcquireReaderLock(_settings.RegistryTimeoutSpan);

                return _mappings.TryGetValue(nanoTypeId, out var result) 
                    ? Task.FromResult<InternalSuperpositionMapping?>(result) 
                    : Task.FromResult<InternalSuperpositionMapping?>(null);
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        public Task Synch(InternalSuperpositionMapping superpositionMapping)
        {
            try
            {
                _rwLock.AcquireWriterLock(_settings.RegistryTimeoutSpan);
                _mappings[superpositionMapping.NanoTypeId] = superpositionMapping;

                return Task.CompletedTask;
            }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }
        }
    }
}
