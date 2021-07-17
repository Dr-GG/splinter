namespace Splinter.Common.Extensions
{
    public static class NanoParameterExtensions
    {
        public static TNanoParameters Cast<TNanoParameters>(this NanoParameters parameters)
            where TNanoParameters : NanoParameters
        {
            var result = parameters as TNanoParameters;

            if (result == null)
            {
                throw new InvalidNanoParametersException(
                    $"Expected the type {typeof(TNanoParameters).FullName} but got {parameters.GetType().FullName}");
            }

            return result;
        }
    }
}
