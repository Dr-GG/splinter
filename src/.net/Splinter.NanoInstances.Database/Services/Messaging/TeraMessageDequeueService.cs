using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Interfaces.Mappers;

namespace Splinter.NanoInstances.Database.Services.Messaging
{
    public class TeraMessageDequeueService : ITeraMessageDequeueService
    {
        private readonly TeraMessagingSettings _settings;
        private readonly TeraDbContext _dbContext;
        private readonly IUnaryMapper<TeraMessageModel, TeraMessage> _messageMapper;

        public TeraMessageDequeueService(
            TeraMessagingSettings settings, 
            TeraDbContext teraDbContext, 
            IUnaryMapper<TeraMessageModel, TeraMessage> messageMapper)
        {
            _settings = settings;
            _dbContext = teraDbContext;
            _messageMapper = messageMapper;
        }

        public async Task<IEnumerable<TeraMessage>> Dequeue(TeraMessageDequeueParameters parameters)
        {
            if (parameters.MaximumNumberOfTeraMessages <= 0)
            {
                return Enumerable.Empty<TeraMessage>();
            }

            var result = new List<TeraMessage>();
            var teraMessages = await GetInitialTeraMessages(
                parameters.TeraId, 
                parameters.MaximumNumberOfTeraMessages);

            foreach (var teraMessage in teraMessages)
            {
                if (!await DequeueMessage(teraMessage))
                {
                    continue;
                }

                var message = _messageMapper.Map(teraMessage);

                result.Add(message);
            }

            return result;
        }

        private async Task<bool> DequeueMessage(TeraMessageModel teraMessage)
        {
            try
            {
                if (CancelTeraMessage(teraMessage))
                {
                    return false;
                }

                await MarkAsDequeued(teraMessage);
            }
            catch (DBConcurrencyException)
            {
                // Ignore the message, because another process modified it.

                return false;
            }

            return true;
        }

        private bool CancelTeraMessage(TeraMessageModel teraMessage)
        {
            if (teraMessage.DequeueCount < _settings.MaximumDequeueRetryCount)
            {
                return false;
            }

            teraMessage.Status = TeraMessageStatus.Cancelled;
            teraMessage.ErrorCode = TeraMessageErrorCode.MaximumDequeueCountReached;
            teraMessage.CompletedTimestamp = DateTime.UtcNow;

            _dbContext.PendingTeraMessages.Remove(teraMessage.Pending);
            _dbContext.SaveChanges();

            return true;
        }

        private async Task MarkAsDequeued(TeraMessageModel teraMessage)
        {
            teraMessage.Status = TeraMessageStatus.Dequeued;
            teraMessage.DequeuedTimestamp = DateTime.UtcNow;
            teraMessage.DequeueCount++;

            await _dbContext.SaveChangesAsync();
        }

        private async Task<IEnumerable<TeraMessageModel>> GetInitialTeraMessages(
            Guid teraId,
            int maximumNumberOfMembers)
        {
            var messageIds = await _dbContext.PendingTeraMessages
                .AsNoTracking()
                .Where(p => p.TeraAgent.TeraId == teraId
                            && p.TeraMessage.Status == TeraMessageStatus.Pending)
                .OrderByDescending(p => p.TeraMessage.Priority)
                .ThenBy(p => p.TeraMessage.LoggedTimestamp)
                .Take(maximumNumberOfMembers)
                .Select(p => p.TeraMessageId)
                .ToListAsync();

            return await _dbContext.TeraMessages
                .Include(m => m.Pending)
                .Include(m => m.SourceTeraAgent)
                .Where(m => messageIds.Contains(m.Id))
                .OrderByDescending(m => m.Priority)
                .ThenBy(m => m.LoggedTimestamp)
                .ToListAsync();
        }
    }
}
