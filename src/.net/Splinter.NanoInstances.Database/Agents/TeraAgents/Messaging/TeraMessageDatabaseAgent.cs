using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Database.Agents.TeraAgents.Messaging;

/// <summary>
/// The default implementation of the ITeraMessageAgent using database implementations and services.
/// </summary>
public class TeraMessageDatabaseAgent : SingletonTeraAgent, ITeraMessageAgent
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// The Nano Type ID.
    /// </summary>
    public static readonly SplinterId NanoTypeId = SplinterIdConstants.TeraMessageAgentNanoTypeId;
    
    /// <summary>
    /// The Nano Instance ID.
    /// </summary>
    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Tera Agent Database Message",
        Version = "1.0.0",
        Guid = new Guid("{807AA709-9838-4BFA-98AF-9D8E45ACBD4C}")
    };

    /// <inheritdoc />
    public override SplinterId TypeId => NanoTypeId;

    /// <inheritdoc />
    public override SplinterId InstanceId => NanoInstanceId;

    /// <inheritdoc />
    protected override bool RegisterInContainer => false;

    /// <inheritdoc />
    protected override bool HasKnowledge => false;

    /// <inheritdoc />
    public async Task<TeraMessageResponse> Send(TeraMessageRelayParameters parameters)
    {
        await using var scope = await NewScope();
        var relayService = await scope.Resolve<ITeraMessageRelayService>();

        return await relayService.Relay(parameters);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TeraMessage>> Dequeue(TeraMessageDequeueParameters parameters)
    {
        await using var scope = await NewScope();
        var dequeueService = await scope.Resolve<ITeraMessageDequeueService>();

        return await dequeueService.Dequeue(parameters);
    }

    /// <inheritdoc />
    public async Task Sync(TeraMessageSyncParameters parameters)
    {
        await using var scope = await NewScope();
        var syncService = await scope.Resolve<ITeraMessageSyncService>();

        await syncService.Sync(parameters);
    }

    /// <inheritdoc />
    protected override async Task SingletonInitialise(NanoInitialisationParameters parameters)
    {
        await base.SingletonInitialise(parameters);
        await RunMessageDisposeTask();
    }

    /// <inheritdoc />
    protected override async Task SingletonDispose(NanoDisposeParameters parameters)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        await base.SingletonDispose(parameters);
    }

    private async Task RunMessageDisposeTask()
    {
        var settings = await Scope.Resolve<TeraMessagingSettings>();
        var cancellationToken = _cancellationTokenSource.Token;

        new Task(() =>
        {
            while (cancellationToken.CanContinue())
            {
                DisposeTeraMessages();

                Thread.Sleep(settings.ExpiredDisposing.MessageDisposalIntervalTimeSpan);
            }

        }).Start();
    }

    private void DisposeTeraMessages()
    {
        using var scope = Scope.StartSync();
        var disposer = scope.ResolveSync<IExpiredTeraMessageDisposeService>();

        disposer.DisposeExpiredMessages();
    }
}