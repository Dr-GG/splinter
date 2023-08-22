using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The database model that holds dependencies between Nano Types and Tera Agent instances.
/// </summary>
[Table("TeraAgentNanoTypeDependencies", Schema = DatabaseSchemaConstants.Superposition)]
public class TeraAgentNanoTypeDependencyModel
{
    /// <summary>
    /// The unique ID of the model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The ID of the referenced TeraAgentModel.
    /// </summary>
    public long TeraAgentId { get; set; }

    /// <summary>
    /// The referenced TeraAgentModel.
    /// </summary>
    public TeraAgentModel TeraAgent { get; set; } = null!;

    /// <summary>
    /// The ID of the referenced NanoTypeModel.
    /// </summary>
    public long NanoTypeId { get; set; }

    /// <summary>
    /// The referenced NanoTypeModel.
    /// </summary>
    public NanoTypeModel NanoType { get; set; } = null!;

    /// <summary>
    /// The number of dependencies between the Nano Type and Tera Agent.
    /// </summary>
    public int NumberOfDependencies { get; set; }
}