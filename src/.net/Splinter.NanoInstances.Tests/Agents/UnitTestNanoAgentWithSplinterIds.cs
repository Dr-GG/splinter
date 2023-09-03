using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.WaveFunctions;

namespace Splinter.NanoInstances.Tests.Agents;

public class UnitTestNanoAgentWithSplinterIds : UnitTestAbstractNanoAgent
{
    public bool InvokedInternalInitialiseNanoReferences { get; private set; }

    public static readonly SplinterId NanoTypeId = new()
    {
        Guid = new Guid("{81CF32F3-FECF-42DD-B9A5-4765D00D4E20}"),
        Name = "Unit Test Nano Type Id",
        Version = "1.0.0"
    };

    public static readonly SplinterId NanoInstanceId = new()
    {
        Guid = new Guid("{F839280A-6AF7-45E2-9CF6-A1CA633031EE}"),
        Name = "Unit Test Nano Instance Id",
        Version = "1.0.0"
    };

    public override SplinterId TypeId => NanoTypeId;
    public override SplinterId InstanceId => NanoInstanceId;

    public void OverrideNanoWaveFunction(INanoWaveFunction nanoWaveFunction)
    {
        NanoWaveFunction = nanoWaveFunction;
    }

    protected override Task InitialiseNanoReferences()
    {
        InvokedInternalInitialiseNanoReferences = true;

        return base.InitialiseNanoReferences();
    }
}