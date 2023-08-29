using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.Services.Messaging;

/// <summary>
/// The default implementation of the IExpiredTeraMessageDisposeService
/// </summary>
public class ExpiredTeraMessageDisposeService : IExpiredTeraMessageDisposeService
{
    private readonly TeraMessagingSettings _settings;
    private readonly TeraDbContext _dbContext;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public ExpiredTeraMessageDisposeService(
        TeraMessagingSettings settings,
        TeraDbContext dbContext)
    {
        _settings = settings;
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public void DisposeExpiredMessages()
    {
        var messages = GetDisposingTeraMessages();

        foreach (var message in messages)
        {
            DisposeExpiredMessage(message);
        }
    }

    private void DisposeExpiredMessage(TeraMessageModel teraMessage)
    {
        try
        {
            teraMessage.Status = TeraMessageStatus.Cancelled;
            teraMessage.ErrorCode = TeraMessageErrorCode.Disposed;
            teraMessage.CompletedTimestamp = DateTime.UtcNow;

            _dbContext.PendingTeraMessages.Remove(teraMessage.Pending);
            _dbContext.SaveChanges();
        }
        catch (DBConcurrencyException)
        {
            // Ignore error, because another process changed it.
        }
    }

    private IEnumerable<TeraMessageModel> GetDisposingTeraMessages()
    {
        var utcNow = DateTime.UtcNow;

        return _dbContext.TeraMessages
            .Include(m => m.Pending)
            .Where(m => (m.Status == TeraMessageStatus.Dequeued
                         || m.Status == TeraMessageStatus.Pending)
                        && m.AbsoluteExpiryTimestamp < utcNow)
            .Take(_settings.ExpiredDisposing.MaximumNumberOfMessagesToDispose)
            .ToList();
    }
}