using Splinter.NanoTypes.Domain.Exceptions.NanoParameters;
using Splinter.NanoTypes.Domain.Parameters;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of extension methods for a NanoParameters instances.
/// </summary>
public static class NanoParameterExtensions
{
    /// <summary>
    /// Casts a NanoParameters instance to another NanoParameters type.
    /// </summary>
    /// <exception cref="InvalidNanoParametersException">
    /// Thrown when the NanoParameters instance could not be successfully cast.
    /// </exception>
    public static TNanoParameters Cast<TNanoParameters>(this NanoParameters parameters)
        where TNanoParameters : NanoParameters
    {
        if (parameters is not TNanoParameters result)
        {
            throw new InvalidNanoParametersException(
                $"Expected the type {typeof(TNanoParameters).FullName} but got {parameters.GetType().FullName}.");
        }

        return result;
    }
}