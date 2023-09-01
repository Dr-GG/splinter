using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Core;
using System;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Interfaces.ServiceScope;
using Splinter.NanoTypes.Interfaces.WaveFunctions;

namespace Splinter.NanoInstances.Tests.Agents;

public class UnitTestTeraAgent : TeraAgent
{
    public bool OverrideHasKnowledge { get; set; } = true;

    public bool OverrideRegisterInContainer { get; set; } = true;

    public SplinterId OverrideTeraAgentNanoTypeId { get; set; } = SplinterIdConstants.TeraDefaultKnowledgeNanoTypeId;

    protected override bool HasKnowledge => OverrideHasKnowledge;

    protected override bool RegisterInContainer => OverrideRegisterInContainer;

    protected override SplinterId TeraKnowledgeNanoTypeId => OverrideTeraAgentNanoTypeId;

    public void OverrideScope(IServiceScope scope)
    {
        Scope = scope;
    }

    public void OverrideWaveFunction(INanoWaveFunction waveFunction)
    {
        NanoWaveFunction = waveFunction;
    }

    public override SplinterId TypeId => new()
    {
        Version = "1.0.0",
        Name = "Test Tera Agent Nano Type",
        Guid = Guid.Parse("{DC54CC94-0704-4D62-BFCA-FDE978722667}")
    };

    public override SplinterId InstanceId => new()
    {
        Version = "1.0.0",
        Name = "Test Tera Agent Nano Instance",
        Guid = Guid.Parse("{A51D3B83-6E83-4DDA-8174-D8A21EAE0121}")
    };
}
