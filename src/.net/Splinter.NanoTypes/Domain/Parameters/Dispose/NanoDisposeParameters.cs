namespace Splinter.NanoTypes.Domain.Parameters.Dispose
{
    public record NanoDisposeParameters : NanoParameters
    {
        public bool Force { get; init; }
    }
}
