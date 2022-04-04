using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoTypes.Domain.Messaging;
using Tenjin.Interfaces.Mappers;

namespace Splinter.NanoInstances.Database.Mappers
{
    public class TeraMessageMapper : IUnaryMapper<TeraMessageModel, TeraMessage>
    {
        public TeraMessage Map(TeraMessageModel source)
        {
            return new TeraMessage
            {
                Id = source.Id,
                Status = source.Status,
                Code = source.Code,
                Priority = source.Priority,
                BatchId = source.BatchId,
                DequeueCount = source.DequeueCount,
                LoggedTimestamp = source.LoggedTimestamp,
                CompletedTimestamp = source.CompletedTimestamp,
                AbsoluteExpiryTimestamp = source.AbsoluteExpiryTimestamp,
                DequeuedTimestamp = source.DequeuedTimestamp,
                Message = source.Message,
                ErrorCode = source.ErrorCode,
                ErrorMessage = source.ErrorMessage,
                SourceTeraId = source.SourceTeraAgent.TeraId
            };
        }
    }
}
