using Splinter.NanoTypes.Domain.Exceptions.NanoParameters;
using Splinter.NanoTypes.Domain.Parameters;

namespace Splinter.NanoInstances.Extensions
{
    public static class NanoParameterExtensions
    {
        public static TNanoParameters Cast<TNanoParameters>(this NanoParameters parameters)
            where TNanoParameters : NanoParameters
        {
            if (parameters is not TNanoParameters result)
            {
                throw new InvalidNanoParametersException(
                    $"Expected the type {typeof(TNanoParameters).FullName} but got {parameters.GetType().FullName}");
            }

            return result;
        }
    }
}
