using System;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoInstances.Default.Tests.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Default.Tests.Agents.Knowledge
{
    public class LanguageTeraAgent : TeraAgent
    {
        public static readonly SplinterId NanoTypeId = LanguageSplinterIds.LanguageNanoTypeId;
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Tera Agent",
            Version = "1.0.0",
            Guid = new Guid("{ACB8D950-9761-464B-A582-D9E1D1F01679}")
        };

        public LanguageTeraAgent(Guid teraId)
        {
            TeraId = teraId;
        }

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;

        protected override SplinterId TeraKnowledgeNanoTypeId => LanguageSplinterIds.KnowledgeNanoTypeId;
    }
}
