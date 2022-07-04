using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Bootstrap;

namespace Splinter.NanoTypes.Interfaces.Bootstrap;

public interface ISplinterBootstrapper
{
    Task Initialise(NanoBootstrapParameters parameters);
}