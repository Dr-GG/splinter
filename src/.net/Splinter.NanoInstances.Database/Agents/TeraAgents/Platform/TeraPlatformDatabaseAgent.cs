using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Database.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraPlatforms;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Termination;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;

namespace Splinter.NanoInstances.Database.Agents.TeraAgents.Platform;

/// <summary>
/// The default implementation of the ITeraPlatform agent using database implementations and services.
/// </summary>
public class TeraPlatformDatabaseAgent : SingletonTeraAgent, ITeraPlatformAgent
{
    private long _operatingSystemId = -1;

    /// <summary>
    /// The Nano Type ID.
    /// </summary>
    public static readonly SplinterId NanoTypeId = SplinterIdConstants.TeraPlatformAgentNanoTypeId;

    /// <summary>
    /// The Nano Instance ID.
    /// </summary>
    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Tera Platform Database Agent",
        Version = "1.0.0",
        Guid = new Guid("{F33DEE7E-2673-4A76-94C8-5FBC7B8C02B7}")
    };

    /// <inheritdoc />
    public override SplinterId TypeId => NanoTypeId;

    /// <inheritdoc />
    public override SplinterId InstanceId => NanoInstanceId;

    /// <inheritdoc />
    public long TeraPlatformId { get; private set; }

    /// <inheritdoc />
    protected override bool RegisterInContainer => false;

    /// <inheritdoc />
    protected override bool HasKnowledge => false;

    /// <inheritdoc />
    protected override async Task SingletonInitialise(NanoInitialisationParameters parameters)
    {
        await base.SingletonInitialise(parameters);

        await RegisterOperatingSystemInformation();
        await RegisterPlatform();
    }

    /// <inheritdoc />
    protected override async Task SingletonTerminate(NanoTerminationParameters parameters)
    {
        await DisablePlatform();

        await base.SingletonTerminate(parameters);
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