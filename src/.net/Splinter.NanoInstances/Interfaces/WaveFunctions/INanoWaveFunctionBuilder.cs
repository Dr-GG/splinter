using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Interfaces.WaveFunctions
{
    public interface INanoWaveFunctionBuilder
    {
        Task Register<TNanoType>(SplinterId nanoTypeId) where TNanoType : INanoAgent;
        Task Register(SplinterId nanoTypeId, Type nanoType);
        Task<INanoWaveFunctionContainer> Build(TimeSpan lockTimeoutTimeSpan);
    }
}
