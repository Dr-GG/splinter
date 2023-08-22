using System.Threading.Tasks;

namespace Splinter.NanoTypes.Default.Interfaces.Services.TeraPlatforms;

/// <summary>
/// The interface of the service responsible for managing and registering Tera platform instances.
/// </summary>
public interface ITeraPlatformManager
{
    /// <summary>
    /// Registers a Tera platform instances.
    /// </summary>
    Task<long> RegisterTeraPlatform(long operatingSystemId);

    /// <summary>
    /// Disables a Tera platform instance.
    /// </summary>
    Task DisableTeraPlatform();
}