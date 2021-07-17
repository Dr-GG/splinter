using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Superposition
{
    public interface ITeraAgentNanoTypeDependencyService
    {
        Task IncrementTeraAgentNanoTypeDependencies(Guid teraId, SplinterId nanoType, int numberOfDependencies = 1);
        Task DecrementTeraAgentNanoTypeDependencies(Guid teraId, SplinterId nanoType, int numberOfDependencies = 1);
        Task DisposeTeraAgentNanoTypeDependencies(Guid teraId);
        Task<Guid?> SignalNanoTypeRecollapses(Guid sourceTeraId, Guid nanoTypeId, IEnumerable<Guid>? teraIds);
    }
}
