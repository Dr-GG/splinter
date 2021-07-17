using System;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.DbContext.Models
{
    [Table("TeraAgents", Schema = DatabaseSchemaConstants.Tera)]
    public class TeraAgentModel
    {
        public long Id { get; set; }
        public Guid TeraId { get; set; }
        public long NanoInstanceId { get; set; }
        public NanoInstanceModel NanoInstance { get; set; } = null!;
        public long TeraPlatformId { get; set; }
        public TeraPlatformModel TeraPlatform { get; set; } = null!;
        public TeraAgentStatus Status { get; set; }
    }
}
