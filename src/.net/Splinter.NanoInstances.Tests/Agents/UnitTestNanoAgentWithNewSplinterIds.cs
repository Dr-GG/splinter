using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Tests.Agents
{
    public class UnitTestNanoAgentWithNewSplinterIds : UnitTestNanoAgentWithSplinterIds
    {
        public new static readonly SplinterId NanoTypeId = new()
        {
            Guid = new Guid("{4FF448F8-2B2B-4E92-B1FB-84CE3C344349}"),
            Name = "Unit Test Nano Type Id",
            Version = "1.0.0"
        };

        public new static readonly SplinterId NanoInstanceId = new()
        {
            Guid = new Guid("{5711A30F-160E-468C-9E7C-2167824238F0}"),
            Name = "Unit Test Nano Instance Id",
            Version = "1.0.0"
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;
    }
}
