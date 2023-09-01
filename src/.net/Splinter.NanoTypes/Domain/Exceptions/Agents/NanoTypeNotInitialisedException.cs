using System;

namespace Splinter.NanoTypes.Domain.Exceptions.Services;

/// <summary>
/// The exception that is generated if a NanoType has not yet been initialised.
/// </summary>
public class NanoTypeNotInitialisedException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeNotInitialisedException() : base()
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeNotInitialisedException(string message) : base(message)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeNotInitialisedException(string message, Exception innerException) : base(message, innerException)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeNotInitialisedException(Type nanoType) : base($"The nano type '{nanoType}' has not yet been initialised.")
    { }
}

/// <summary>
/// The exception that is generated if a NanoType has not yet been initialised.
/// </summary>
public class NanoTypeNotInitialiseException<TType> : NanoTypeNotInitialisedException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeNotInitialiseException() : base(typeof(TType))
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeNotInitialiseException(string message) : base(message)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeNotInitialiseException(string message, Exception innerException) : base(message, innerException)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeNotInitialiseException(Type NanoTypeType) : base(NanoTypeType)
    { }
}