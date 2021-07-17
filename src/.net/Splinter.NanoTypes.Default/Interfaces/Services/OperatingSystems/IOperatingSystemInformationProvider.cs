using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Platforms;

namespace Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems
{
    public interface IOperatingSystemInformationProvider
    {
        Task<OperatingSystemInformation> GetOperatingSystemInformation();
        Task<string> GetFrameworkDescription();
    }
}
