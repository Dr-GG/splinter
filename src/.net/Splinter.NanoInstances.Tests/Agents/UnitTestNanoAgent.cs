using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Tests.Agents
{
    public class UnitTestNanoAgent : UnitTestAbstractNanoAgent
    {
        public override SplinterId TypeId => throw new Exception();
        public override SplinterId InstanceId => throw new Exception();
    }
}
