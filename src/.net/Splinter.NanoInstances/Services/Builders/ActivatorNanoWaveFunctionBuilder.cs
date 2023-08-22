using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoInstances.Interfaces.WaveFunctions;
using Splinter.NanoInstances.Services.Containers;
using Splinter.NanoInstances.Utilities;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Services.Builders;

/// <summary>
/// The default implementation of the INanoWaveFunctionBuilder.
/// </summary>
public class ActivatorNanoWaveFunctionBuilder : INanoWaveFunctionBuilder
{
    private readonly IDictionary<Guid, Type> _nanoTypeMap = new Dictionary<Guid, Type>();

    /// <inheritdoc />
    public Task Register<TNanoAgent>(SplinterId nanoTypeId) where TNanoAgent : INanoAgent
    {
        return Register(nanoTypeId, typeof(TNanoAgent));
    }

    /// <inheritdoc />
    public Task Register(SplinterId nanoTypeId, Type nanoType)
    {
        if (!NanoTypeUtilities.IsNanoInstance(nanoType))
        {
            throw new InvalidNanoInstanceException(nanoType);
        }

        _nanoTypeMap[nanoTypeId.Guid] = nanoType;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<INanoWaveFunctionContainer> Build(TimeSpan lockTimeoutTimeSpan)
    {
        return Task.FromResult<INanoWaveFunctionContainer>(
            new ActivatorNanoWaveFunctionContainer(lockTimeoutTimeSpan, _nanoTypeMap));
    }
}