using System.Diagnostics.CodeAnalysis;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of extension methods for the INanoReference interface.
/// </summary>
public static class NanoReferenceExtensions
{
    /// <summary>
    /// Determines if the reference within an INanoReference is not null or not empty.
    /// </summary>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this INanoReference? nanoReference)
    {
        return nanoReference is {HasReference: true};
    }

    /// <summary>
    /// Determines if the reference within an INanoReference is null or empty.
    /// </summary>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this INanoReference? nanoReference)
    {
        return nanoReference == null || nanoReference.HasNoReference;
    }

    /// <summary>
    /// Casts the reference in an INanoReference to a specified INanoAgent type.
    /// </summary>
    public static TNanoAgent Typed<TNanoAgent>(this INanoReference nanoReference)
        where TNanoAgent : INanoAgent
    {
        if (nanoReference.HasNoReference)
        {
            throw new InvalidNanoInstanceException("No nano instance exists in nano reference.");
        }

        if (nanoReference.Reference is not TNanoAgent agent)
        {
            throw new InvalidNanoInstanceException(
                $"The nano instance in the nano reference is not of the type {typeof(TNanoAgent)}.");
        }

        return agent;
    }
}