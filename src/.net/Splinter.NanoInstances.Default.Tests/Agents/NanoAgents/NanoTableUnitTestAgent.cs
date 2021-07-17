using System;
using Splinter.NanoInstances.Agents.NanoAgents;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Default.Tests.Agents.NanoAgents
{
    public class NanoTableUnitTestAgent : NanoAgent
    {
        private readonly SplinterId _splinterId;

        public NanoTableUnitTestAgent(Guid nanoTypeId)
        {
            _splinterId = new SplinterId
            {
                Guid = nanoTypeId,
                Name = "Nano Table Unit Test Agent",
                Version = "1.0.0"
            };
        }

        public override SplinterId TypeId => _splinterId;
        public override SplinterId InstanceId => _splinterId;

        public bool IsReference(INanoReference reference)
        {
            if (reference.HasNoReference)
            {
                return false;
            }

            var thisHashCode = GetHashCode();
            var refHashCode = reference.Reference.GetHashCode();

            return thisHashCode == refHashCode;
        }
    }
}
