﻿using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Core;
using System;

namespace Splinter.NanoInstances.Tests.Agents;

public class UnitTestTeraAgent : TeraAgent
{
    public bool OverrideHasKnowledge { get; set; } = true;

    protected override bool HasKnowledge => OverrideHasKnowledge;

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