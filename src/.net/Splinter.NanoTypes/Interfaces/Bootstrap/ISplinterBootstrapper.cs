using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Bootstrap;

namespace Splinter.NanoTypes.Interfaces.Bootstrap;

/// <summary>
/// The interface that is responsible for bootstrapping a Splinter environment.
/// </summary>
public interface ISplinterBootstrapper
{
    /// <summary>
    /// Initialises the Splinter environment with specified parameters.
    /// </summary>
    Task Initialise(INanoBootstrapParameters parameters);
}