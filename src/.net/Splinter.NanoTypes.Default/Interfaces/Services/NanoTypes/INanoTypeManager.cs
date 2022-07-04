using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;

public interface INanoTypeManager
{
    Task RegisterNanoType(INanoAgent agent);
    Task<long?> GetNanoTypeId(SplinterId nanoTypeId);
    Task<long?> GetNanoInstanceId(SplinterId nanoInstanceId);
}