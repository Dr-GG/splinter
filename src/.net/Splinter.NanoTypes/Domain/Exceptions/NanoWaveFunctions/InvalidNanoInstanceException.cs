using System;

namespace Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions
{
    public class InvalidNanoInstanceException : SplinterException
    {
        public InvalidNanoInstanceException(string message) : base(message)
        { }

        public InvalidNanoInstanceException(Type type) :
            base($"The type '{type.FullName} is not a nano instance")
        { }
    }
}
