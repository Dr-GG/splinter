using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splinter.Domain.Exceptions.NanoParameters
{
    public class InvalidNanoParametersException : SplinterException
    {
        public InvalidNanoParametersException(string message) : base(message)
        { }
    }
}
