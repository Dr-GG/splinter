namespace Splinter.Domain.Initialisation
{
    public record NanoInitialisationParameters : NanoParameters
    {
        public object ServiceScope { get; set; } = null!;
    }
}
