using System;
using System.Threading.Tasks;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge.Languages;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.Applications.Test.Agents.NanoAgents.Knowledge.Languages
{
    public class GibberishLanguageAgent : NanoAgent, ILanguageAgent
    {
        public static readonly SplinterId NanoTypeId = LanguageSplinterIds.LanguageNanoTypeId;
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Gibberish Language Agent",
            Version = "1.0.0",
            Guid = new Guid("{B25C0F84-3432-4C7F-A13A-30760DA9B755}")
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;

        public Task<string> SayHello()
        {
            return Task.FromResult(LanguageConstants.Gibberish.Hello);
        }

        public Task<string> SayTest()
        {
            return Task.FromResult(LanguageConstants.Gibberish.Test);
        }

        public Task<string> SayGoodbye()
        {
            return Task.FromResult(LanguageConstants.Gibberish.Goodbye);
        }
    }
}
