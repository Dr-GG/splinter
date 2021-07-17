using System;

namespace Splinter.Domain.Exceptions.NanoWaveFunctions
{
    public class InvalidNanoInstanceException : SplinterException
    {
        public InvalidNanoInstanceException(Type type) :
            base($"The type '{type.FullName} is not a nano instance")
        { }

        public InvalidNanoInstanceException(string message) : base(message)
        { }
    }
}
