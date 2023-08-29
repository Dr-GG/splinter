using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Default.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoInstances.Default.Services.Messaging;

/// <summary>
/// The default implementation of the ITeraMessageQueue interface.
/// </summary>
public class TeraMessageQueue : ITeraMessageQueue
{
    private readonly TeraMessagingSettings _settings;
    private readonly Queue<TeraMessage> _queue = new();
    private ITeraAgent? _teraAgent;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeraMessageQueue(TeraMessagingSettings settings)
    {
        _settings = settings;
    }

    private ITeraAgent Parent => _teraAgent.AssertReturnGetterValue(() => new NanoServiceNotInitialisedException(typeof(TeraMessageQueue)));

    /// <inheritdoc />
    public Task Initialise(ITeraAgent teraAgent)
    {
        _teraAgent = teraAgent;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<bool> HasNext()
    {
        if (_queue.Count > 0)
        {
            return true;
        }

        return await GetTeraMessageBatch();
    }

    /// <inheritdoc />
    public Task<TeraMessage> Next()
    {
        if (_queue.Count == 0)
        {
            throw new InvalidNanoServiceOperationException("No new tera message could be dequeued.");
        }

        return Task.FromResult(_queue.Dequeue());
    }

    /// <inheritdoc />
    public async Task Terminate(TeraMessageSyncParameters parameters)
    {
        await SplinterEnvironment.TeraMessageAgent.Sync(parameters);
    }

    private async Task<bool> GetTeraMessageBatch()
    {
        var parameters = new TeraMessageDequeueParameters
        {
            MaximumNumberOfTeraMessages = _settings.MaximumTeraAgentFetchCount,
            TeraId = Parent.TeraId
        };

        var messages = await SplinterEnvironment.TeraMessageAgent.Dequeue(parameters);

        foreach (var message in messages)
        {
            _queue.Enqueue(message);
        }

        return _queue.Count > 0;
    }
}