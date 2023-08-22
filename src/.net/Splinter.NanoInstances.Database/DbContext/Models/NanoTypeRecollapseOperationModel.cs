using System;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Database.Domain.Constants;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The Nano Type Recollapse Operation database model.
/// </summary>
[Table("NanoTypeRecollapseOperations", Schema = DatabaseSchemaConstants.Superposition)]
public class NanoTypeRecollapseOperationModel
{
    /// <summary>
    /// The unique ID of the database model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The unique ID of the operation.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the Nano Type being recollapsed.
    /// </summary>
    public long NanoTypeId { get; set; }

    /// <summary>
    /// The referenced NanoTypeModel.
    /// </summary>
    public NanoTypeModel NanoType { get; set; } = null!;

    /// <summary>
    /// The created timestamp.
    /// </summary>
    public DateTime CreatedTimestamp { get; set; }

    /// <summary>
    /// The number of expected recollapses.
    /// </summary>
    public long NumberOfExpectedRecollapses { get; set; }

    /// <summary>
    /// The number of successful recollapses.
    /// </summary>
    public long NumberOfSuccessfulRecollapses { get; set; }

    /// <summary>
    /// The number of failed recollapses.
    /// </summary>
    public long NumberOfFailedRecollapses { get; set; }
}