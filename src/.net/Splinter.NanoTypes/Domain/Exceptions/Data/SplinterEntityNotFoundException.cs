namespace Splinter.NanoTypes.Domain.Exceptions.Data;

/// <summary>
/// The exception that is generated if an entity within Splinter was not found.
/// </summary>
public class SplinterEntityNotFoundException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SplinterEntityNotFoundException(string entityType, object entityId) : base($"Could not find the {entityType} with ID {entityId}.")
    { }
}