namespace Splinter.Applications.Test.Domain.Languages
{
    public record LanguagePack
    {
        public string Hello { get; init; } = string.Empty;
        public string Test { get; init; } = string.Empty;
        public string Goodbye { get; init; } = string.Empty;
    }
}
