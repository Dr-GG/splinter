using System.ComponentModel.DataAnnotations.Schema;

namespace Splinter.NanoInstances.Database.DbContext.Models
{
    [Table("PendingTeraMessages", Schema = "tera")]
    public class PendingTeraMessageModel
    {
        public long Id { get; set; }
        public long TeraAgentId { get; set; }
        public TeraAgentModel TeraAgent { get; set; } = null!;
        public long TeraMessageId { get; set; }
        public TeraMessageModel TeraMessage { get; set; } = null!;
    }
}
