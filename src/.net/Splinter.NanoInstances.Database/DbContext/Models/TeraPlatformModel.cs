using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.DbContext.Models
{
    [Table("TeraPlatforms", Schema = DatabaseSchemaConstants.Platform)]
    public class TeraPlatformModel
    {
        public long Id { get; set; }
        public Guid TeraId { get; set; }
        public TeraPlatformStatus Status { get; set; }
        public long OperatingSystemId { get; set; }
        public OperatingSystemModel OperatingSystem { get; set; } = null!;
        [MaxLength(200)]
        public string FrameworkDescription { get; set; } = string.Empty;

	}
}
