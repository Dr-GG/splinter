using System;

namespace Splinter.NanoTypes.Default.Domain.Settings.Messaging;

/// <summary>
/// The settings used when disposing of expired TeraMessage instances.
/// </summary>
public class ExpiredTeraMessageDisposeSettings
{
    /// <summary>
    /// The maximum number of expired TeraMessage instances to dispose of.
    /// </summary>
    public int MaximumNumberOfMessagesToDispose { get; set; }

    /// <summary>
    /// The time span interval at which expired TeraMessage instances are disposed of.
    /// </summary>
    public TimeSpan MessageDisposalIntervalTimeSpan { get; set; }

    /// <summary>
    /// The 
    /// </summary>
    public TimeSpan DefaultExpiryTimeSpan { get; set; }
}