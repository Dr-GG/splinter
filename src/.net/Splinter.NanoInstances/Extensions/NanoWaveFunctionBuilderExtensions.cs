using Splinter.NanoInstances.Interfaces.WaveFunctions;
using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Utilities;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Extensions;

public static class NanoWaveFunctionBuilderExtensions
{
    public static Task Register(this INanoWaveFunctionBuilder builder, string typeUrl)
    {
        var type = Type.GetType(typeUrl);

        if (type == null)
        {
            throw new InvalidNanoTypeException($"Could not find nano type {typeUrl}");
        }

        return builder.Register(type);
    }

    public static Task Register<TNanoType>(this INanoWaveFunctionBuilder builder)
        where TNanoType : INanoAgent
    {
        return builder.Register(typeof(TNanoType));
    }

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
                $"Could not find the static '{typeof(SplinterId).FullName}' with the name '{SplinterIdConstants.NanoTypeId}'");
        }

        await builder.Register(nanoTypeId, nanoType);
    }
}