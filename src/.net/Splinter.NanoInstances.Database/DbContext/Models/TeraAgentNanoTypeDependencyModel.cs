using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;

namespace Splinter.NanoInstances.Database.DbContext.Models
{
    [Table("TeraAgentNanoTypeDependencies", Schema = DatabaseSchemaConstants.Superposition)]
    public class TeraAgentNanoTypeDependencyModel
    {
        public long Id { get; set; }
        public long TeraAgentId { get; set; }
        public TeraAgentModel TeraAgent { get; set; } = null!;
        public long NanoTypeId { get; set; }
        public NanoTypeModel NanoType { get; set; } = null!;
        public int NumberOfDependencies { get; set; }
    }
}
