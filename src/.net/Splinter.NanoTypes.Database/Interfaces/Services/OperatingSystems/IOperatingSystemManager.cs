using System.Threading.Tasks;

namespace Splinter.NanoTypes.Database.Interfaces.Services.OperatingSystems;

/// <summary>
/// The interface that handles operating system information.
/// </summary>
public interface IOperatingSystemManager
{
    /// <summary>
    /// Registers the OS information in the database and returns a unique ID/handle.
    /// </summary>
    Task<long> RegisterOperatingSystemInformation();
}