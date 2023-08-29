using System;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoTypes.Domain.Parameters.Initialisation;

/// <summary>
/// The nano parameters used to initialise a Nano Type.
/// </summary>
public record NanoInitialisationParameters : INanoParameters
{
    /// <summary>
    /// The flag indicating if the Nano Type should be registered.
    /// </summary>
    /// <remarks>
    /// This flag is only applicable to Tera Type instances.
    /// </remarks>
    public bool Register { get; init; } = true;

    /// <summary>
    /// The flag indicating if the INanoTable of the Tera Agent should be resolved.
    /// </summary>
    /// <remarks>
    /// This flag is only applicable to Tera Type instances.
    /// </remarks>
    public bool RegisterNanoTable { get; init; } = true;

    /// <summary>
    /// The flag indicating if the nano references of a Nano Type should be registered.
    /// </summary>
    public bool RegisterNanoReferences { get; init; } = true;

    /// <summary>
    /// The ID of the Tera Type to be used.
    /// </summary>
    public Guid? TeraId { get; init; }

    /// <summary>
    /// The IServiceScope to be used for resolving services.
    /// </summary>
    public IServiceScope ServiceScope { get; init; } = null!;
}