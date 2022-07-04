using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Database.Interfaces.Services.Registration;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Registration;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;

namespace Splinter.NanoInstances.Database.Agents.TeraAgents.Registry;

public class TeraRegistryDatabaseAgent : SingletonTeraAgent, ITeraRegistryAgent
{
    public static readonly SplinterId NanoTypeId = SplinterIdConstants.TeraRegistryAgentNanoTypeId;
    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Tera Registry Database Tera Instance",
        Version = "1.0.0",
        Guid = new Guid("{4DE29985-1A6B-4C86-A32E-AE769A6BEC0D}")
    };

    public override SplinterId TypeId => NanoTypeId;
    public override SplinterId InstanceId => NanoInstanceId;

    protected override bool RegisterInContainer => false;
    protected override bool HasKnowledge => false;

    public async Task<Guid> Register(TeraAgentRegistrationParameters parameters)
    {
        await using var scope = await NewScope();
        var service = await scope.Resolve<ITeraAgentRegistrationService>();
        var teraPlatformId = TeraPlatformAgent.TeraPlatformId;

        return await service.Register(teraPlatformId, parameters);
    }

    public async Task Dispose(TeraAgentDisposeParameters parameters)
    {
        await using var scope = await NewScope();
        var service = await scope.Resolve<ITeraAgentRegistrationService>();

        await service.Dispose(parameters);
    }
}