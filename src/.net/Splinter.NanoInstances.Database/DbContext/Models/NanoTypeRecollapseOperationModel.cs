using System;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;

namespace Splinter.NanoInstances.Database.DbContext.Models
{
    [Table("NanoTypeRecollapseOperations", Schema = DatabaseSchemaConstants.Superposition)]
    public class NanoTypeRecollapseOperationModel
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public long NanoTypeId { get; set; }
        public NanoTypeModel NanoType { get; set; } = null!;
        public DateTime CreatedTimestamp { get; set; }
        public long NumberOfExpectedRecollapses { get; set; }
        public long NumberOfSuccessfulRecollapses { get; set; }
        public long NumberOfFailedRecollapses { get; set; }
	}
}
