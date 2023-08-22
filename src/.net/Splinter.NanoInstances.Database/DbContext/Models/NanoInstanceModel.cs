using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The Nano Instance database model.
/// </summary>
[Table("NanoInstances", Schema = DatabaseSchemaConstants.Nano)]
public class NanoInstanceModel
{
    /// <summary>
    /// The unique ID of the model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The referenced Nano Type ID.
    /// </summary>
    public long NanoTypeId { get; set; }

    /// <summary>
    /// The referenced NanoTypeModel.
    /// </summary>
    public NanoTypeModel NanoType { get; set; } = null!;

    /// <summary>
    /// The unique ID of the Nano Instance.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The name of the Nano Instance.
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The version of the Nano Instance.
    /// </summary>
    [MaxLength(100)]
    public string Version { get; set; } = string.Empty;
}