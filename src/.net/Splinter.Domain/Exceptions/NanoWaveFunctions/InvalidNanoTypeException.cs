using System;

namespace Splinter.Domain.Exceptions.NanoWaveFunctions
{
    public class InvalidNanoTypeException : SplinterException
    {
        public InvalidNanoTypeException(Type type) : 
            base($"The type '{type.FullName} is not a nano type")
        { }

        public InvalidNanoTypeException(string message) : base(message)
        { }
    }
}
