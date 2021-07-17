using System;

namespace Splinter.Domain.Collapse
{
    public record NanoCollapseParameters : NanoParameters
    {
        public Guid TeraAgentId { get; set; }
        public Guid NanoTypeId { get; set; }
    }
}
