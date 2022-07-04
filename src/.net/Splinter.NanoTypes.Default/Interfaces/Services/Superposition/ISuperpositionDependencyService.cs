using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Default.Interfaces.Services.Superposition;

public interface ISuperpositionDependencyService
{
    Task Register(SplinterId nanoTypeId, SplinterId nanoInstanceId);
    Task Register(Guid teraAgentId, Guid nanoTypeId);
}