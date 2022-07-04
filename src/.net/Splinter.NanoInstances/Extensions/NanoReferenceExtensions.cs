using System.Diagnostics.CodeAnalysis;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Extensions;

public static class NanoReferenceExtensions
{
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this INanoReference? nanoReference)
    {
        return nanoReference is {HasReference: true};
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this INanoReference? nanoReference)
    {
        return nanoReference == null || nanoReference.HasNoReference;
    }

    public static TNanoAgent Typed<TNanoAgent>(this INanoReference nanoReference)
        where TNanoAgent : INanoAgent
    {
        if (nanoReference.HasNoReference)
        {
            throw new InvalidNanoInstanceException("No nano instance exists in nano reference");
        }

        if (nanoReference.Reference is not TNanoAgent agent)
        {
            throw new InvalidNanoInstanceException(
                $"The nano instance in the nano reference is not of the type {typeof(TNanoAgent)}");
        }

        return agent;
    }
}