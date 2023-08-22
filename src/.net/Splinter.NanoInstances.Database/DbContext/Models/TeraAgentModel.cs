using System;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The Tera Agent database model.
/// </summary>
[Table("TeraAgents", Schema = DatabaseSchemaConstants.Tera)]
public class TeraAgentModel
{
    /// <summary>
    /// The unique ID of the model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The unique ID of the Tera Agent.
    /// </summary>
    public Guid TeraId { get; set; }

    /// <summary>
    /// The ID of the referenced NanoInstanceModel.
    /// </summary>
    public long NanoInstanceId { get; set; }

    /// <summary>
    /// The referenced NanoInstanceModel.
    /// </summary>
    public NanoInstanceModel NanoInstance { get; set; } = null!;

    /// <summary>
    /// The ID of the referenced TeraPlatformModel.
    /// </summary>
    public long TeraPlatformId { get; set; }

    /// <summary>
    /// The referenced TeraPlatformModel.
    /// </summary>
    public TeraPlatformModel TeraPlatform { get; set; } = null!;

    /// <summary>
    /// The current status of the Tera Agent.
    /// </summary>
    public TeraAgentStatus Status { get; set; }
}