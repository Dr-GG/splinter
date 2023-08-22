using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Interfaces.WaveFunctions;

/// <summary>
/// The interface of a builder facilitating the building an INanoWaveFunctionContainer.
/// </summary>
public interface INanoWaveFunctionBuilder
{
    /// <summary>
    /// Registers a Nano Type ID.
    /// </summary>
    Task Register<TNanoType>(SplinterId nanoTypeId) where TNanoType : INanoAgent;

    /// <summary>
    /// Registers a Nano Type ID and a Type that reflects a Nano Type.
    /// </summary>
    Task Register(SplinterId nanoTypeId, Type nanoType);

    /// <summary>
    /// Builds the INanoWaveFunctionBuilder instance.
    /// </summary>
    Task<INanoWaveFunctionContainer> Build(TimeSpan lockTimeoutTimeSpan);
}