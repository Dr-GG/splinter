using System;

namespace Splinter.NanoTypes.Domain.Exceptions.Services
{
    public class NanoServiceNotInitialisedException : SplinterException
    {
        public NanoServiceNotInitialisedException(Type nanoServiceType) : 
            base($"The nano service '{nanoServiceType}' has not yet been initialised")
        { }
    }
}
