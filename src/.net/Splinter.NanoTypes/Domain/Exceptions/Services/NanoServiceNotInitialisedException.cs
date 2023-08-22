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
    public NanoServiceNotInitialisedException(Type nanoServiceType) : base($"The nano service '{nanoServiceType}' has not yet been initialised.")
    { }
}