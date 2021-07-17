using System;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Default.Tests.Agents.NanoAgents
{
    public class UnitTestNanoAgent : NanoAgent
    {
        public override SplinterId TypeId => throw new NotImplementedException();
        public override SplinterId InstanceId => throw new NotImplementedException();
    }
}
