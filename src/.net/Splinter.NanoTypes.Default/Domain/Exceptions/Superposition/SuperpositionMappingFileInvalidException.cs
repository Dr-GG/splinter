using System;
using Splinter.NanoTypes.Domain.Exceptions;

namespace Splinter.NanoTypes.Default.Domain.Exceptions.Superposition
{
    public class SuperpositionMappingFileInvalidException : SplinterException
    {
        public SuperpositionMappingFileInvalidException(string message) : base(message)
        { }

        public SuperpositionMappingFileInvalidException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
