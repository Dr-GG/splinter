using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Platforms;

namespace Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems;

/// <summary>
/// The interface of a service that provides all necessary operating system information.
/// </summary>
public interface IOperatingSystemInformationProvider
{
    /// <summary>
    /// Gets OS information of the currently executing OS.
    /// </summary>
    Task<OperatingSystemInformation> GetOperatingSystemInformation();

    /// <summary>
    /// Gets the description of the runtime framework that Splinter is running on.
    /// </summary>
    Task<string> GetFrameworkDescription();
}