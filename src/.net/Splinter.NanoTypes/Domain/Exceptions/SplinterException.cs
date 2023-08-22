using System;

namespace Splinter.NanoTypes.Domain.Exceptions;

/// <summary>
/// The base class for all Splinter specific exceptions.
/// </summary>
public class SplinterException : Exception
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SplinterException()
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SplinterException(string message) : base(message)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SplinterException(string message, Exception innerException) 
        : base(message, innerException)
    { }
}