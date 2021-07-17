using System;

namespace Splinter.Domain.Exceptions
{
    public class SplinterException : Exception
    {
        public SplinterException(string message) : base(message)
        { }

        public SplinterException()
        { }

        public SplinterException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}
