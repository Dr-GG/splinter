using System;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.Models.Messaging
{
    public record TeraMessageAgentRecipient
    {
        public long TeraAgentId { get; init; }
        public Guid TeraAgentTeraId { get; init; }
        public TeraAgentStatus Status { get; init; }
    }
}
