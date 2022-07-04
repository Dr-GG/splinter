using System.Threading.Tasks;

namespace Splinter.NanoTypes.Default.Interfaces.Services.TeraPlatforms;

public interface ITeraPlatformManager
{
    Task<long> RegisterTeraPlatform(long operatingSystemId);
    Task DisableTeraPlatform();
}