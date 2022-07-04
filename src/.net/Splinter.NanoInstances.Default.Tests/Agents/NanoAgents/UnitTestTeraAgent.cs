using System;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Default.Tests.Agents.NanoAgents;

public class UnitTestTeraAgent : TeraAgent
{
    public override SplinterId TypeId => new()
    {
        Name = "Unit Test Tera Agent",
        Version = "1.0.0",
        Guid = new Guid("{2B4D88A3-F5B0-4F3E-8BC6-A89003FCE59A}")
    };

    public override SplinterId InstanceId => new()
    {
        Name = "Unit Test Tera Agent",
        Version = "1.0.0",
        Guid = new Guid("{C5F765DB-9C90-4F14-92F4-E3B584347B05}")
    };
}