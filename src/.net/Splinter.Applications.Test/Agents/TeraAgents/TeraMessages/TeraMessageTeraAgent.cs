using System;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.Applications.Test.Agents.TeraAgents.TeraMessages
{
    public class TeraMessageTeraAgent : TeraAgent
    {
        public static readonly SplinterId NanoTypeId = TeraMessageSplinterIds.TeraTypeId;
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Tera Agent",
            Version = "1.0.0",
            Guid = new Guid("{102904AD-CD6A-4FFD-B95E-C78C7006D655}")
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;

        protected override SplinterId TeraKnowledgeNanoTypeId => TeraMessageSplinterIds.KnowledgeNanoTypeId;
    }
}
