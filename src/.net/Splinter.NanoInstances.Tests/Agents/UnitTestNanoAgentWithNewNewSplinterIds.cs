using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Tests.Agents
{
    public class UnitTestNanoAgentWithNewNewSplinterIds : UnitTestNanoAgentWithNewSplinterIds
    {
        public new static readonly SplinterId NanoTypeId = new()
        {
            Guid = new Guid("{8E113484-72EA-4023-B18B-2C0B83AE951E}"),
            Name = "Unit Test Nano Type Id",
            Version = "1.0.0"
        };

        public new static readonly SplinterId NanoInstanceId = new()
        {
            Guid = new Guid("{AC0E3B99-3CF2-4AC4-8651-FC88720E1B63}"),
            Name = "Unit Test Nano Instance Id",
            Version = "1.0.0"
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;
    }
}
