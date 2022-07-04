using System;

namespace Splinter.NanoTypes.Domain.Exceptions.ServiceScope;

public class ServiceUnresolvedException : SplinterException
{
    public ServiceUnresolvedException(Type type) : 
        base($"Could not resolve the service type {type.FullName}")
    { }
}