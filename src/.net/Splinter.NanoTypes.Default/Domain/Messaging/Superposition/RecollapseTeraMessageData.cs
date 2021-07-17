using System;

namespace Splinter.NanoTypes.Default.Domain.Messaging.Superposition
{
    public record RecollapseTeraMessageData
    {
        public Guid NanoTypeRecollapseOperationId { get; init; }
        public Guid NanoTypeId { get; init; }
    }
}
