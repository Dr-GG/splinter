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

public class TeraMessageDatabaseAgent : SingletonTeraAgent, ITeraMessageAgent
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public static readonly SplinterId NanoTypeId = SplinterIdConstants.TeraMessageAgentNanoTypeId;
    public static readonly SplinterId NanoInstanceId = new()
    {
        Name = "Tera Agent Database Message",
        Version = "1.0.0",
        Guid = new Guid("{807AA709-9838-4BFA-98AF-9D8E45ACBD4C}")
    };

    public override SplinterId TypeId => NanoTypeId;
    public override SplinterId InstanceId => NanoInstanceId;

    protected override bool RegisterInContainer => false;
    protected override bool HasKnowledge => false;

    public async Task<TeraMessageResponse> Send(TeraMessageRelayParameters parameters)
    {
        await using var scope = await NewScope();
        var relayService = await scope.Resolve<ITeraMessageRelayService>();

        return await relayService.Relay(parameters);
    }

    public async Task<IEnumerable<TeraMessage>> Dequeue(TeraMessageDequeueParameters parameters)
    {
        await using var scope = await NewScope();
        var dequeueService = await scope.Resolve<ITeraMessageDequeueService>();

        return await dequeueService.Dequeue(parameters);
    }

    public async Task Sync(TeraMessageSyncParameters parameters)
    {
        await using var scope = await NewScope();
        var syncService = await scope.Resolve<ITeraMessageSyncService>();

        await syncService.Sync(parameters);
    }

    protected override async Task SingletonInitialise(NanoInitialisationParameters parameters)
    {
        await base.SingletonInitialise(parameters);
        await RunMessageDisposeTask();
    }

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

                Thread.Sleep(settings.Disposing.MessageDisposalIntervalTimeSpan);
            }

        }).Start();
    }

    private void DisposeTeraMessages()
    {
        using var scope = Scope.StartSync();
        var disposer = scope.ResolveSync<ITeraMessageDisposeService>();

        disposer.DisposeMessages();
    }
}