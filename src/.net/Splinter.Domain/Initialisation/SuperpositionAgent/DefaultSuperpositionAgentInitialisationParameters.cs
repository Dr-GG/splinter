namespace Splinter.Domain.Initialisation.SuperpositionAgent
{
    public record DefaultSuperpositionAgentInitialisationParameters : NanoInitialisationParameters
    {
        public string SuperpositionMappingPath { get; set; } = string.Empty;
    }
}
