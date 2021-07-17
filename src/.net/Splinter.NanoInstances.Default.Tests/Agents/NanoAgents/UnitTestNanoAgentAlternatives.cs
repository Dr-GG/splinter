using System;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Default.Tests.Agents.NanoAgents
{
    public static class UnitTestNanoAgentAlternativeNanoTypeIds
    {
        public const string NanoTypeId1 = "{3A9FAE24-7296-4ED9-B1F9-32A73F2DF94A}";
        public const string NanoTypeId2 = "{AA66C102-C4A9-4B72-8111-185DF51622DC}";
        public const string NanoTypeId3 = "{1654457F-FDBB-4BB0-98E4-1CE2D8B3B41D}";
        public const string NanoTypeId4 = "{1A633377-CDB9-4507-98A6-E2B8640ECD6E}";
    }

    public class UnitTestNanoAgentAlternative1 : NanoAgent
    {
        public static readonly SplinterId NanoTypeId = new()
        {
            Name = "Unit Test Alt 1",
            Version = "1",
            Guid = new Guid(UnitTestNanoAgentAlternativeNanoTypeIds.NanoTypeId1)
        };
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Unit Test Alt 1",
            Version = "1",
            Guid = new Guid("{E1DCAF86-AF85-43E8-8720-956113A930D7}")
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class UnitTestNanoAgentAlternative2 : NanoAgent
    {
        public static readonly SplinterId NanoTypeId = new()
        {
            Name = "Unit Test Alt 2",
            Version = "1",
            Guid = new Guid(UnitTestNanoAgentAlternativeNanoTypeIds.NanoTypeId2)
        };
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Unit Test Alt 2",
            Version = "1",
            Guid = new Guid("{549865F4-0110-46B3-B5DD-8E49226B7CEE}")
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class UnitTestNanoAgentAlternative3 : NanoAgent
    {
        public static readonly SplinterId NanoTypeId = new()
        {
            Name = "Unit Test Alt 3",
            Version = "1",
            Guid = new Guid(UnitTestNanoAgentAlternativeNanoTypeIds.NanoTypeId3)
        };
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Unit Test Alt 3",
            Version = "1",
            Guid = new Guid("{E7EC07F2-5F7C-4EFB-80B2-ADD953E4FDC5}")
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;
    }

    public class UnitTestNanoAgentAlternative4 : NanoAgent
    {
        public static readonly SplinterId NanoTypeId = new()
        {
            Name = "Unit Test Alt 4",
            Version = "1",
            Guid = new Guid(UnitTestNanoAgentAlternativeNanoTypeIds.NanoTypeId4)
        };
        public static readonly SplinterId NanoInstanceId = new()
        {
            Name = "Unit Test Alt 4",
            Version = "1",
            Guid = new Guid("{87BCC159-DB10-4302-ABB7-01970C38411E}")
        };

        public override SplinterId TypeId => NanoTypeId;
        public override SplinterId InstanceId => NanoInstanceId;
    }
}
