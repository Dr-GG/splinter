using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Superposition
{
    public interface IRecollapseNanoTypeService
    {
        Task<int> Recollapse(INanoAgent parent, TeraMessage recollapseMessage);
    }
}
