using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoTypes.Database.Interfaces.Services.Messaging;

public interface ITeraMessageDequeueService
{
    Task<IEnumerable<TeraMessage>> Dequeue(TeraMessageDequeueParameters parameters);
}