namespace Splinter.NanoTypes.Domain.Exceptions.Data;

public class EntityNotFoundException : SplinterException
{
    public EntityNotFoundException(string entityType, object entityId) : 
        base($"Could not find the {entityType} with ID {entityId}")
    { }
}