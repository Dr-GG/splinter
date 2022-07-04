using System;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoTypes.Domain.Parameters.Initialisation;

public record NanoInitialisationParameters : NanoParameters
{
    public bool Register { get; init; } = true;
    public bool RegisterNanoTable { get; init; } = true;
    public bool RegisterNanoReferences { get; init; } = true;
    public Guid? TeraId { get; init; }
    public IServiceScope ServiceScope { get; init; } = null!;
}