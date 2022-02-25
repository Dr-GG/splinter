using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Services.Superposition
{
    public class NanoReference : INanoReference
    {
        private INanoAgent? _reference;

        public bool HasNoReference => _reference == null;

        public bool HasReference => _reference != null;

        public INanoAgent Reference
        {
            get
            {
                if (_reference == null)
                {
                    throw new InvalidNanoInstanceException("No nano instance reference initialised");
                }

                return _reference;
            }
        }

        public Task Initialise(INanoAgent? reference)
        {
            _reference = reference;

            return Task.CompletedTask;
        }
    }
}
