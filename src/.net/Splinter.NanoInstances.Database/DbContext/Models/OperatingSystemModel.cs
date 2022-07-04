using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.DbContext.Models;

[Table("OperatingSystems", Schema = DatabaseSchemaConstants.Platform)]
public class OperatingSystemModel
{
    public long Id { get; set; }
    public ProcessorArchitecture ProcessorArchitecture { get; set; }
    public OperatingSystem Type { get; set; }
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}