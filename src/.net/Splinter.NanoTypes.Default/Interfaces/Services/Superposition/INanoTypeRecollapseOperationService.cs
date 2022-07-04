using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Superposition;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Superposition;

public interface INanoTypeRecollapseOperationService
{
    Task Sync(NanoRecollapseOperationParameters parameters);
}