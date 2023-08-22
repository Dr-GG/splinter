using Splinter.NanoInstances.Interfaces.WaveFunctions;
using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Utilities;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of extension methods for the INanoWaveFunctionBuilder interface.
/// </summary>
public static class NanoWaveFunctionBuilderExtensions
{
    /// <summary>
    /// Registers a Type as a Nano Type based on the fully qualified path name of a Type.
    /// </summary>
    public static Task Register(this INanoWaveFunctionBuilder builder, string typeUrl)
    {
        var type = Type.GetType(typeUrl);

        return type == null 
            ? throw new InvalidNanoTypeException($"Could not find nano type {typeUrl}.") 
            : builder.Register(type);
    }

    /// <summary>
    /// Registers an INanoAgent type as a Nano Type.
    /// </summary>
    public static Task Register<TNanoType>(this INanoWaveFunctionBuilder builder)
        where TNanoType : INanoAgent
    {
        return builder.Register(typeof(TNanoType));
    }

    /// <summary>
    /// Registers a Type that represents an INanoAgent as a Nano Type.
    /// </summary>
    public static async Task Register(
        this INanoWaveFunctionBuilder builder,
        Type nanoType)
    {
        if (!NanoTypeUtilities.IsNanoInstance(nanoType))
        {
            throw new InvalidNanoInstanceException(nanoType);
        }

        NanoTypeUtilities.GetSplinterIds(nanoType, 
            out var nanoTypeId, 
            out _);

        if (nanoTypeId == null)
        {
            throw new InvalidNanoTypeException(
                $"Could not find the static '{typeof(SplinterId).FullName}' with the name '{SplinterIdConstants.NanoTypeIdPropertyName}'.");
        }

        await builder.Register(nanoTypeId, nanoType);
    }
}