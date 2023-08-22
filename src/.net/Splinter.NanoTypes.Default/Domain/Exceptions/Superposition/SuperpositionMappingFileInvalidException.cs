using System;
using Splinter.NanoTypes.Domain.Exceptions;

namespace Splinter.NanoTypes.Default.Domain.Exceptions.Superposition;

/// <summary>
/// The exception that is generated when a superposition mapping file is invalid.
/// </summary>
public class SuperpositionMappingFileInvalidException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SuperpositionMappingFileInvalidException(string message) : base(message)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SuperpositionMappingFileInvalidException(string message, Exception innerException)
        : base(message, innerException)
    { }
}