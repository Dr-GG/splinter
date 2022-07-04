using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Tests.Agents;

public class UnitTestNanoAgentWithOneSplinterId : UnitTestAbstractNanoAgent
{
    public static readonly SplinterId NanoTypeId = new()
    {
        Guid = new Guid("{413B20F0-0227-4E79-8677-DAA205918D08}"),
        Name = "Unit Test Nano Type Id",
        Version = "1.0.0"
    };

    public override SplinterId TypeId => NanoTypeId;
    public override SplinterId InstanceId => throw new NotSupportedException();
}