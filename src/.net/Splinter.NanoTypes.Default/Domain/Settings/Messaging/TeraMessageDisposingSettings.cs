using System;

namespace Splinter.NanoTypes.Default.Domain.Settings.Messaging
{
    public class TeraMessageDisposeSettings
    {
        public int MaximumNumberOfMessagesToDispose { get; set; }
        public TimeSpan MessageDisposalIntervalTimeSpan { get; set; }
        public TimeSpan DefaultExpiryTimeSpan { get; set; }
    }
}
