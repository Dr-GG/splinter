using System;

namespace Splinter.Domain.Exceptions.SuperpositionMappings
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
