using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Database.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraPlatforms;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;

namespace Splinter.NanoInstances.Database.Agents.TeraAgents.Platform;

public class TeraPlatformDatabaseAgent : SingletonTeraAgent, ITeraPlatformAgent
{
    private long _operatingSystemId = -1;

    public static readonly SplinterId NanoTypeId = SplinterIdConstants.TeraPlatformAgentNanoTypeId;
    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Tera Platform Database Agent",
        Version = "1.0.0",
        Guid = new Guid("{F33DEE7E-2673-4A76-94C8-5FBC7B8C02B7}")
    };

    public override SplinterId TypeId => NanoTypeId;
    public override SplinterId InstanceId => NanoInstanceId;
    public long TeraPlatformId { get; private set; }

    protected override bool RegisterInContainer => false;
    protected override bool HasKnowledge => false;

    protected override async Task SingletonInitialise(NanoInitialisationParameters parameters)
    {
        await base.SingletonInitialise(parameters);

        await RegisterOperatingSystemInformation();
        await RegisterPlatform();
    }

    protected override async Task SingletonDispose(NanoDisposeParameters parameters)
    {
        await DisablePlatform();

        await base.SingletonDispose(parameters);
    }

    private async Task RegisterOperatingSystemInformation()
    {
        await using var scope = await NewScope();
        var manager = await scope.Resolve<IOperatingSystemManager>();

        _operatingSystemId = await manager.RegisterOperatingSystemInformation();
    }

    private async Task RegisterPlatform()
    {
        await using var scope = await NewScope();
        var manager = await scope.Resolve<ITeraPlatformManager>();

        TeraPlatformId = await manager.RegisterTeraPlatform(_operatingSystemId);
    }

    private async Task DisablePlatform()
    {
        await using var scope = await NewScope();
        var manager = await scope.Resolve<ITeraPlatformManager>();

        await manager.DisableTeraPlatform();
    }
}