using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The Tera Platform database model.
/// </summary>
[Table("TeraPlatforms", Schema = DatabaseSchemaConstants.Platform)]
public class TeraPlatformModel
{
    /// <summary>
    /// The unique ID of the model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The Tera Agent ID of the ITeraPlatformAgent.
    /// </summary>
    public Guid TeraId { get; set; }

    /// <summary>
    /// The current state of the agent.
    /// </summary>
    public TeraPlatformStatus Status { get; set; }

    /// <summary>
    /// The ID of the referenced OperatingSystemModel.
    /// </summary>
    public long OperatingSystemId { get; set; }

    /// <summary>
    /// The referenced OperatingSystemModel.
    /// </summary>
    public OperatingSystemModel OperatingSystem { get; set; } = null!;

    /// <summary>
    /// The runtime framework description.
    /// </summary>
    [MaxLength(200)]
    public string FrameworkDescription { get; set; } = string.Empty;

}