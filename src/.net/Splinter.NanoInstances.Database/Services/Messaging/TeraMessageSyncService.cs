using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Database.Services.Messaging;

public class TeraMessageSyncService : ITeraMessageSyncService
{
    private readonly TeraMessagingSettings _settings;
    private readonly TeraDbContext _dbContext;

    public TeraMessageSyncService(
        TeraMessagingSettings settings,
        TeraDbContext dbContext)
    {
        _settings = settings;
        _dbContext = dbContext;
    }

    public async Task Sync(TeraMessageSyncParameters parameters)
    {
        var messages = await GetMessages(parameters);

        foreach (var message in messages)
        {
            var sync = parameters.Syncs.Single(s => s.TeraMessageId == message.Id);

            await Sync(message, sync);
        }
    }

    private static void Reset(TeraMessageModel message)
    {
        if (message.Status.EqualsAny(
                TeraMessageStatus.Dequeued, 
                TeraMessageStatus.Pending))
        {
            return;
        }

        message.ErrorCode = null;
        message.ErrorMessage = null;
        message.ErrorStackTrace = null;
    }

    private void RemovePending(TeraMessageModel message)
    {
        if (message.Status.EqualsAny(
                TeraMessageStatus.Pending,
                TeraMessageStatus.Dequeued))
        {
            _dbContext.PendingTeraMessages.Remove(message.Pending);
        }
    }

    private async Task Sync(TeraMessageModel message, TeraMessageSyncParameter parameter)
    {
        var synced = false;

        for (var i = 0; i < _settings.MaximumSyncRetryCount && !synced; i++)
        {
            try
            {
                await InternalSync(message, parameter);

                synced = true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ignore the error as another processed changed it.
            }
        }
    }

    private async Task InternalSync(TeraMessageModel message, TeraMessageSyncParameter parameter)
    {
        if (message.Status == TeraMessageStatus.Completed)
        {
            return;
        }

        Reset(message);
        RemovePending(message);

        message.CompletedTimestamp = parameter.CompletionTimestamp;

        if (parameter.ErrorCode.HasValue
            || parameter.ErrorMessage.IsNotNullOrEmpty()
            || parameter.ErrorStackTrace.IsNotNullOrEmpty())
        {
            message.Status = TeraMessageStatus.Failed;
            message.ErrorCode = parameter.ErrorCode ?? TeraMessageErrorCode.Unknown;
            message.ErrorMessage = parameter.ErrorMessage;
            message.ErrorStackTrace = parameter.ErrorStackTrace;
        }
        else
        {
            message.Status = TeraMessageStatus.Completed;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<IEnumerable<TeraMessageModel>> GetMessages(
        TeraMessageSyncParameters parameters)
    {
        var messageIds = parameters.Syncs.Select(s => s.TeraMessageId).ToList();

        return await _dbContext.TeraMessages
            .Include(m => m.Pending)
            .Where(m => messageIds.Contains(m.Id))
            .ToListAsync();
    }
}