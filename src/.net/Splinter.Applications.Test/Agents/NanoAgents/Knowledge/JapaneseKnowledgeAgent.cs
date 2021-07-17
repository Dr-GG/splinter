using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.NanoInstances.Default.Agents.NanoAgents.Knowledge;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;

namespace Splinter.Applications.Test.Agents.NanoAgents.Knowledge
{
    public class JapaneseKnowledgeAgent : TeraKnowledgeAgent, ILanguageKnowledgeAgent
    {
        private readonly ICollection<string> _saidPhrases = new List<string>();

        public new static readonly SplinterId NanoTypeId = LanguageSplinterIds.KnowledgeNanoTypeId;
        public new static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Japanese Knowledge Agent",
            Version = "1.0.0",
            Guid = new Guid("{609DC264-BE26-4288-8308-B71CF5E60327}")
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;
        public IEnumerable<string> SaidPhrases => _saidPhrases.ToList();

        public override async Task Execute(TeraAgentExecutionParameters parameters)
        {
            await base.Execute(parameters);

            _saidPhrases.Clear();
            _saidPhrases.Add(LanguageConstants.Japanese.Hello);
            _saidPhrases.Add(LanguageConstants.Japanese.Test);
            _saidPhrases.Add(LanguageConstants.Japanese.Goodbye);
        }
    }
}
