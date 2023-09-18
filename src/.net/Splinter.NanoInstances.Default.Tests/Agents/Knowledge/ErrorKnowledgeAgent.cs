using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Agents.NanoAgents.Knowledge;
using Splinter.NanoTypes.Domain.Messaging;

namespace Splinter.NanoInstances.Default.Tests.Agents.Knowledge;

public class ErrorKnowledgeAgent : TeraKnowledgeAgent
{
    public const string ErrorMessage = "This is not supported";

    protected override Task<bool> ProcessTeraMessage(TeraMessage teraMessage)
    {
        throw new NotSupportedException(ErrorMessage);
    }
}
