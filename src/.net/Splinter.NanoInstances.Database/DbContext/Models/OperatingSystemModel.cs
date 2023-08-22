using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The operating system database model.
/// </summary>
[Table("OperatingSystems", Schema = DatabaseSchemaConstants.Platform)]
public class OperatingSystemModel
{
    /// <summary>
    /// The unique ID of the model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The processor architecture type.
    /// </summary>
    public ProcessorArchitecture ProcessorArchitecture { get; set; }

    /// <summary>
    /// The operating system type.
    /// </summary>
    public OperatingSystem Type { get; set; }

    /// <summary>
    /// The logical description of the OS.
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}