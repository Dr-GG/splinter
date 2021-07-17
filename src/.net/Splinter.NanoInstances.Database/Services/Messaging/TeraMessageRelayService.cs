using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoInstances.Database.Extensions;
using Splinter.NanoInstances.Database.Models.Messaging;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoInstances.Database.Services.Messaging
{
    public class TeraMessageRelayService : ITeraMessageRelayService
    {
        private readonly TeraMessagingSettings _settings;
        private readonly TeraDbContext _dbContext;

        public TeraMessageRelayService(
            TeraMessagingSettings settings, 
            TeraDbContext teraDbContext)
        {
            _settings = settings;
            _dbContext = teraDbContext;
        }

        public async Task<TeraMessageResponse> Relay(TeraMessageRelayParameters parameters)
        {
            var batchId = Guid.NewGuid();
            var recipients = (await GetRecipientStatuses(parameters)).ToList();
            var relayedMessages = await RelayMessage(batchId, parameters, recipients);

            return new TeraMessageResponse
            {
                BatchId = batchId,
                TeraAgentMessageIds = relayedMessages,
                TeraIdsDisposed = GetTeraAgentsDisposedCount(recipients),
                TeraIdsNotFounds = GetTeraAgentsNotFoundCount(parameters, recipients)
            };
        }

        private async Task<IEnumerable<TeraAgentMessageResponse>> RelayMessage(
            Guid batchId,
            TeraMessageRelayParameters parameters,
            IEnumerable<TeraMessageAgentRecipient> recipients)
        {
            var enumeratedRecipients = recipients.ToList();
            var messageModels = (await GetTeraMessageModels(batchId, parameters, enumeratedRecipients)).ToList();

            await _dbContext.TeraMessages.AddRangeAsync(messageModels);
            await _dbContext.SaveChangesAsync();
            await AddMessagesAsPending(messageModels);

            return ConvertToResponses(enumeratedRecipients, messageModels);
        }

        private static IEnumerable<TeraAgentMessageResponse> ConvertToResponses(
            IEnumerable<TeraMessageAgentRecipient> recipients,
            IEnumerable<TeraMessageModel> messages)
        {
            return messages.Select(m => new TeraAgentMessageResponse
            {
                MessageId = m.Id,
                TeraId = recipients.Single(r => r.TeraAgentId == m.RecipientTeraAgentId).TeraAgentTeraId
            }).ToList();
        }

        private async Task<IEnumerable<TeraMessageModel>> GetTeraMessageModels(
            Guid batchId,
            TeraMessageRelayParameters parameters,
            IEnumerable<TeraMessageAgentRecipient> recipients)
        {
            var sourceAgentId = (await _dbContext.GetTeraAgent(parameters.SourceTeraId)).Id;
            var absoluteExpiry = parameters.AbsoluteExpiryTimeSpan ?? _settings.Disposing.DefaultExpiryTimeSpan;
            var absoluteExpiryTimestamp = DateTime.UtcNow.Add(absoluteExpiry);

            return recipients
                .Where(r => r.Status != TeraAgentStatus.Disposed)
                .Select(r => new TeraMessageModel
                {
                    BatchId = batchId,
                    Priority = parameters.Priority,
                    Code = parameters.Code,
                    Message = parameters.Message,
                    SourceTeraAgentId = sourceAgentId,
                    RecipientTeraAgentId = r.TeraAgentId,
                    Status = TeraMessageStatus.Pending,
                    AbsoluteExpiryTimestamp = absoluteExpiryTimestamp,
                    LoggedTimestamp = DateTime.UtcNow
                })
                .ToList();
        }

        private async Task AddMessagesAsPending(IEnumerable<TeraMessageModel> messages)
        {
            var pendingModels = messages.Select(m => new PendingTeraMessageModel
            {
                TeraAgentId = m.RecipientTeraAgentId,
                TeraMessageId = m.Id
            }).ToList();

            await _dbContext.AddRangeAsync(pendingModels);
            await _dbContext.SaveChangesAsync();
        }

        private static IEnumerable<Guid> GetTeraAgentsNotFoundCount(
            TeraMessageRelayParameters parameters, 
            IEnumerable<TeraMessageAgentRecipient> recipients)
        {
            return parameters.RecipientTeraIds
                .Where(mId => recipients.All(r => r.TeraAgentTeraId != mId))
                .ToList();
        }

        private static IEnumerable<Guid> GetTeraAgentsDisposedCount(IEnumerable<TeraMessageAgentRecipient> recipients)
        {
            return recipients
                .Where(r => r.Status == TeraAgentStatus.Disposed)
                .Select(r => r.TeraAgentTeraId)
                .ToList();
        }

        private async Task<IEnumerable<TeraMessageAgentRecipient>> GetRecipientStatuses(
            TeraMessageRelayParameters parameters)
        {
            var guidIds = parameters.RecipientTeraIds.ToList();

            return await _dbContext.TeraAgents
                .Where(t => guidIds.Contains(t.TeraId))
                .Select(t => new TeraMessageAgentRecipient
                {
                    TeraAgentId = t.Id,
                    TeraAgentTeraId = t.TeraId,
                    Status = t.Status
                })
                .ToListAsync();
        }
    }
}
