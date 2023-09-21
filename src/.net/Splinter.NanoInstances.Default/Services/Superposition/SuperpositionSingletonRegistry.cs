﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Termination;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Default.Services.Superposition;

/// <summary>
/// The default implementation of the ISuperpositionSingletonRegistry interface.
/// </summary>
public class SuperpositionSingletonRegistry : ISuperpositionSingletonRegistry
{
    private readonly ReaderWriterLock _rwLock = new();
    private readonly IDictionary<Guid, INanoAgent> _singletonMap = new Dictionary<Guid, INanoAgent>();
    private readonly SuperpositionSettings _settings;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SuperpositionSingletonRegistry(SuperpositionSettings settings)
    {
        _settings = settings;
    }

    /// <inheritdoc />
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

                await TerminateSingletonReference(reference);
            }

            _singletonMap[nanoTypeId] = singleton;

            return singleton;
        }
        finally
        {
            _rwLock.ReleaseWriterLock();
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            _rwLock.AcquireWriterLock(_settings.RegistryTimeoutSpan);

            foreach (var singleton in _singletonMap)
            {
                await TerminateSingletonReference(singleton.Value);
            }

            _singletonMap.Clear();

            GC.SuppressFinalize(this);
        }
        finally
        {
            _rwLock.ReleaseWriterLock();
        }
    }

    private static async Task TerminateSingletonReference(INanoAgent nanoAgent)
    {
        var parameters = new NanoTerminationParameters
        {
            Force = true
        };

        await nanoAgent.Terminate(parameters);
    }
}