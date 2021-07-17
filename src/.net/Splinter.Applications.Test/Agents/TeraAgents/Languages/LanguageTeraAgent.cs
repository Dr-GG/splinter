using System;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.Applications.Test.Agents.TeraAgents.Languages
{
    public class LanguageTeraAgent : TeraAgent
    {
        public static readonly SplinterId NanoTypeId = LanguageSplinterIds.TeraTypeId;
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Tera Agent",
            Version = "1.0.0",
            Guid = new Guid("{ACB8D950-9761-464B-A582-D9E1D1F01679}")
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;

        protected override SplinterId TeraKnowledgeNanoTypeId => LanguageSplinterIds.KnowledgeNanoTypeId;
    }
}
