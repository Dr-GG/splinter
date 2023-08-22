using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Superposition;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Superposition;

/// <summary>
/// The interface of the service responsible for the administration of Nano Type recollapse operations.
/// </summary>
public interface INanoTypeRecollapseOperationService
{
    /// <summary>
    /// Executes the synchronises with an existing Nano Type recollapse operation.
    /// </summary>
    Task Sync(NanoRecollapseOperationParameters parameters);
}