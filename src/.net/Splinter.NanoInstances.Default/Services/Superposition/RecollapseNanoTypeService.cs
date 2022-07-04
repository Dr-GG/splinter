using System;
using System.Linq;
using System.Threading.Tasks;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Default.Domain.Messaging.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Default.Services.Superposition;

public class RecollapseNanoTypeService : IRecollapseNanoTypeService
{
    public async Task<int> Recollapse(INanoAgent parent, TeraMessage recollapseMessage)
    {
        if (parent.HolonType == HolonType.Nano && parent.HasNoTeraParent)
        {
            return 0;
        }

        if (parent.HasNoNanoTable)
        {
            return 0;
        }

        var message = recollapseMessage.JsonMessage<RecollapseTeraMessageData>();

        if (message == null)
        {
            return 0;
        }

        var references = (await parent.NanoTable.Fetch(message.NanoTypeId))
            .Where(r => r.HasReference)
            .ToList();

        foreach (var reference in references)
        {
            await Recollapse(parent, reference, message.NanoTypeId);
        }

        return references.Count;
    }

    private static async Task Recollapse(
        INanoAgent parent, 
        INanoReference reference,
        Guid nanoTypeId)
    {
        var newNanoAgent = await Recollapse(parent, nanoTypeId);
        var oldReference = reference.Reference;

        try
        {
            await oldReference.Lock();

            if (newNanoAgent != null && reference.HasReference)
            {
                await newNanoAgent.Synch(oldReference);
            }

            await reference.Initialise(newNanoAgent);
            await InitialiseNewNanoAgent(reference);
        }
        finally
        {
            var disposeParameters = new NanoDisposeParameters();

            await oldReference.Unlock();
            await oldReference.Dispose(disposeParameters);
        }
    }

    private static async Task<INanoAgent?> Recollapse(INanoAgent parent, Guid nanoTypeId)
    {
        var collapseParameters = new NanoCollapseParameters
        {
            Initialise = false,
            TeraAgentId = parent.TeraParent.TeraId,
            NanoTypeId = nanoTypeId
        };

        return await parent.Collapse(collapseParameters);
    }

    private static async Task InitialiseNewNanoAgent(INanoReference nanoReference)
    {
        if (nanoReference.HasNoReference)
        {
            return;
        }

        var teraParent = nanoReference.Reference.TeraParent;
        var initParameters = new NanoInitialisationParameters
        {
            TeraId = teraParent.TeraId,
            ServiceScope = await teraParent.Scope.Start()
        };

        await nanoReference.Reference.Initialise(initParameters);
    }
}