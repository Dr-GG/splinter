using System;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;

namespace Splinter.NanoTypes.Domain.Parameters.Collapse
{
    public record NanoCollapseParameters : NanoParameters
    {
        public bool Initialise { get; init; } = true;
        public Guid? TeraAgentId { get; init; }
        public Guid NanoTypeId { get; init; }
        public NanoInitialisationParameters? Initialisation { get; init; }
    }
}
