using System;

namespace Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;

/// <summary>
/// The exception that is generated when an invalid Nano Instance was collapsed or retrieved.
/// </summary>
public class InvalidNanoInstanceException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public InvalidNanoInstanceException(string message) : base(message)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public InvalidNanoInstanceException(Type type) : base($"The type '{type.FullName} is not a nano instance.")
    { }
}