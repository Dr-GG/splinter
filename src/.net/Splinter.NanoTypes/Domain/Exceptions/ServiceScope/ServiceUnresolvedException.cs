using System;

namespace Splinter.NanoTypes.Domain.Exceptions.ServiceScope;

/// <summary>
/// The exception that is generated when a service could not be resolved.
/// </summary>
public class ServiceUnresolvedException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public ServiceUnresolvedException(Type type) : base($"Could not resolve the service type {type.FullName}.")
    { }
}