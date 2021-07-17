using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents;

namespace Splinter.NanoTypes.Interfaces.Services.Superposition
{
    public interface INanoReference
    {
        bool HasNoReference { get; }
        bool HasReference { get; }
        INanoAgent Reference { get; }
        
        Task Initialise(INanoAgent? reference);
    }
}
