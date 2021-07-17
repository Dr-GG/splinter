using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoInstances.Default.Tests.Models
{
    public class TeraLanguageAgentsSuite
    {
        public ITeraAgent AfrikaansAgent { get; set; } = null!;
        public ITeraAgent EnglishAgent { get; set; } = null!;
        public ITeraAgent FrenchAgent { get; set; } = null!;
        public ITeraAgent GermanAgent { get; set; } = null!;
        public ITeraAgent SpanishAgent { get; set; } = null!;
    }
}
