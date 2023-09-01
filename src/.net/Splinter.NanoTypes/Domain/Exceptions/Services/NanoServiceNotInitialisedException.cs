using System;

namespace Splinter.NanoTypes.Domain.Exceptions.Services;

/// <summary>
/// The exception that is generated if a NanoService has not yet been initialised.
/// </summary>
public class NanoServiceNotInitialisedException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoServiceNotInitialisedException() : base()
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoServiceNotInitialisedException(string message) : base(message)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoServiceNotInitialisedException(string message, Exception innerException) : base(message, innerException)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoServiceNotInitialisedException(Type nanoServiceType) : base($"The nano service '{nanoServiceType}' has not yet been initialised.")
    { }
}

/// <summary>
/// The exception that is generated if a NanoService has not yet been initialised.
/// </summary>
public class NanoServiceNotInitialiseException<TType>: NanoServiceNotInitialisedException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoServiceNotInitialiseException() : base(typeof(TType))
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoServiceNotInitialiseException(string message) : base(message)
    { }
    
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoServiceNotInitialiseException(string message, Exception innerException) : base(message, innerException)
    { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoServiceNotInitialiseException(Type nanoServiceType) : base(nanoServiceType)
    { }
}