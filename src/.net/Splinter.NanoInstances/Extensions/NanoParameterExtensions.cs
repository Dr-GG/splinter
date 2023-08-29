using Splinter.NanoTypes.Domain.Exceptions.NanoParameters;
using Splinter.NanoTypes.Domain.Parameters;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of extension methods for a INanoParameters instances.
/// </summary>
public static class NanoParameterExtensions
{
    /// <summary>
    /// Casts a INanoParameters instance to another INanoParameters type.
    /// </summary>
    /// <exception cref="InvalidNanoParametersException">
    /// Thrown when the INanoParameters instance could not be successfully cast.
    /// </exception>
    public static TNanoParameters Cast<TNanoParameters>(this INanoParameters parameters)
        where TNanoParameters : INanoParameters
    {
        if (parameters is not TNanoParameters result)
        {
            throw new InvalidNanoParametersException(
                $"Expected the type {typeof(TNanoParameters).FullName} but got {parameters.GetType().FullName}.");
        }

        return result;
    }
}