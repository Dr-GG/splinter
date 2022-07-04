using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;

public interface INanoTypeCache
{
    void RegisterNanoTypeId(SplinterId nanoType, long id);
    void RegisterNanoInstanceId(SplinterId nanoInstance, long id);
    bool TryGetNanoTypeId(SplinterId nanoType, out long id);
    bool TryGetNanoInstanceId(SplinterId nanoInstance, out long id);
}