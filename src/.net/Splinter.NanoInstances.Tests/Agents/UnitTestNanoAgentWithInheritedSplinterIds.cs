using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Tests.Agents;

public class UnitTestNanoAgentWithInheritedSplinterIds : UnitTestNanoAgentWithOneSplinterId
{
    public static readonly SplinterId NanoInstanceId = new()
    {
        Guid = new Guid("{6C1DF681-89D3-489A-88C3-7C37AB64A5D9}"),
        Name = "Unit Test Nano Instance Id",
        Version = "1.0.0"
    };
}