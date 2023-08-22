using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The Nano Type database model.
/// </summary>
[Table("NanoTypes", Schema = DatabaseSchemaConstants.Nano)]
public class NanoTypeModel
{
    /// <summary>
    /// The unique ID of the model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The unique ID of the Nano Type.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The name of the Nano Type.
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The version of the Nano Type.
    /// </summary>
    [MaxLength(100)]
    public string Version { get; set; } = string.Empty;
}