﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Interfaces.WaveFunctions;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Services.Containers;

public class ActivatorNanoWaveFunctionContainer : INanoWaveFunctionContainer
{
    private readonly ReaderWriterLock _rwLock = new();
    private readonly TimeSpan _lockTimeoutTimeSpan;
    private readonly IDictionary<Guid, Type> _nanoTypeMap;

    public ActivatorNanoWaveFunctionContainer(
        TimeSpan lockTimeoutTimeSpan,
        IDictionary<Guid, Type> nanoTypeMap)
    {
        _lockTimeoutTimeSpan = lockTimeoutTimeSpan;
        _nanoTypeMap = nanoTypeMap;
    }

    public Task<INanoAgent?> Collapse(NanoCollapseParameters collapseParameters)
    {
        try
        {
            _rwLock.AcquireReaderLock(_lockTimeoutTimeSpan);

            return _nanoTypeMap.TryGetValue(
                collapseParameters.NanoTypeId, out var type)
                ? Task.FromResult(Activator.CreateInstance(type) as INanoAgent)
                : Task.FromResult<INanoAgent?>(null);
        }
        finally
        {
            _rwLock.ReleaseReaderLock();
        }
    }

    public Task Synch(Guid nanoTypeId, Type type)
    {
        try
        {
            _rwLock.AcquireWriterLock(_lockTimeoutTimeSpan);

            _nanoTypeMap[nanoTypeId] = type;
        }
        finally
        {
            _rwLock.ReleaseWriterLock();
        }

        return Task.CompletedTask;
    }
}