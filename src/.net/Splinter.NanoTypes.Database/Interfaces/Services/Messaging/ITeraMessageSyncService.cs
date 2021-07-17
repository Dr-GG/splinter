using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoTypes.Database.Interfaces.Services.Messaging
{
    public interface ITeraMessageSyncService
    {
        Task Sync(TeraMessageSyncParameters parameters);
    }
}
