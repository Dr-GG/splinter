using System.Threading.Tasks;

namespace Splinter.NanoTypes.Database.Interfaces.Services.OperatingSystems
{
    public interface IOperatingSystemManager
    {
        Task<long> RegisterOperatingSystemInformation();
    }
}
