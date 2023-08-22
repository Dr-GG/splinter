using System.ComponentModel.DataAnnotations.Schema;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The database model that holds pending TeraMessageModel instances.
/// </summary>
[Table("PendingTeraMessages", Schema = "tera")]
public class PendingTeraMessageModel
{
    /// <summary>
    /// The unique ID of the model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The ID of the TeraAgentModel the TeraMessageModel is assigned to.
    /// </summary>
    public long TeraAgentId { get; set; }

    /// <summary>
    /// The referenced TeraAgentModel.
    /// </summary>
    public TeraAgentModel TeraAgent { get; set; } = null!;

    /// <summary>
    /// The ID of the TeraMessageModel pending processing.
    /// </summary>
    public long TeraMessageId { get; set; }

    /// <summary>
    /// The referenced TeraMessageModel.
    /// </summary>
    public TeraMessageModel TeraMessage { get; set; } = null!;
}