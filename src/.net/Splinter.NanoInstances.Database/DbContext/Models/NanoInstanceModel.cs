using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;

namespace Splinter.NanoInstances.Database.DbContext.Models
{
    [Table("NanoInstances", Schema = DatabaseSchemaConstants.Nano)]
    public class NanoInstanceModel
    {
        public long Id { get; set; }
        public long NanoTypeId { get; set; }
        public NanoTypeModel NanoType { get; set; } = null!;
        public Guid Guid { get; set; }
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Version { get; set; } = string.Empty;
    }
}
