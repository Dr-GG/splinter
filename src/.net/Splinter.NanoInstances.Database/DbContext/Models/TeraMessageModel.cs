using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.DbContext.Models;

/// <summary>
/// The Tera Message database model.
/// </summary>
[Table("TeraMessages", Schema = "tera")]
public class TeraMessageModel
{
    /// <summary>
    /// The unique ID of the model.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The execution code of the message.
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// The priority of the message.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// The number dequeue attempts on the message.
    /// </summary>
    public int DequeueCount { get; set; }

    /// <summary>
    /// The unique batch ID that the message belongs to.
    /// </summary>
    public Guid BatchId { get; set; }

    /// <summary>
    /// The current state of the message.
    /// </summary>
    public TeraMessageStatus Status { get; set; }

    /// <summary>
    /// The attached error code of the message, if any.
    /// </summary>
    public TeraMessageErrorCode? ErrorCode { get; set; }

    /// <summary>
    /// The ID of the sender TeraAgentModel.
    /// </summary>
    public long SourceTeraAgentId { get; set; }

    /// <summary>
    /// The referenced sender TeraAgentModel.
    /// </summary>
    public TeraAgentModel SourceTeraAgent { get; set; } = null!;

    /// <summary>
    /// The ID of the recipient TeraAgentModel.
    /// </summary>
    public long RecipientTeraAgentId { get; set; }

    /// <summary>
    /// The referenced recipient TeraAgentModel.
    /// </summary>
    public TeraAgentModel RecipientTeraAgent { get; set; } = null!;

    /// <summary>
    /// The e-tag of the message.
    /// </summary>
    [Timestamp]
    public byte[] ETag { get; set; } = null!;

    /// <summary>
    /// The content of the message.
    /// </summary>
    [MaxLength]
    public string? Message { get; set; }

    /// <summary>
    /// The attached error message, if any.
    /// </summary>
    [MaxLength]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The attached error stack trace, if any.
    /// </summary>
    [MaxLength]
    public string? ErrorStackTrace { get; set; }

    /// <summary>
    /// The expiry timestamp of the message.
    /// </summary>
    public DateTime AbsoluteExpiryTimestamp { get; set; }

    /// <summary>
    /// The logged/create timestamp of the message.
    /// </summary>
    public DateTime LoggedTimestamp { get; set; }

    /// <summary>
    /// The last dequeue timestamp of the message.
    /// </summary>
    public DateTime? DequeuedTimestamp { get; set; }

    /// <summary>
    /// The completed timestamp of the message.
    /// </summary>
    public DateTime? CompletedTimestamp { get; set; }

    /// <summary>
    /// The referenced PendingTeraMessageModel.
    /// </summary>
    public PendingTeraMessageModel Pending { get; set; } = null!;
}