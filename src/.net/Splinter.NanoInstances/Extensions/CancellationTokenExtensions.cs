using System.Threading;

namespace Splinter.NanoInstances.Extensions
{
    public static class CancellationTokenExtensions
    {
        public static bool CanContinue(this CancellationToken cancellationToken)
        {
            return !cancellationToken.IsCancellationRequested;
        }
    }
}
