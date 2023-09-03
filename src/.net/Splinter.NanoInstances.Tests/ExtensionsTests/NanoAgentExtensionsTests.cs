using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Termination;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoInstances.Tests.ExtensionsTests;

public class NanoAgentExtensionsTests
{
    [Test]
    public async Task Terminate_WhenInvoked_ShouldCalAppropriateMethod()
    {
        var mockNanoAgent = new Mock<INanoAgent>();

        mockNanoAgent
            .Setup(x => x.Terminate(
                It.IsAny<NanoTerminationParameters>()))
            .Returns(Task.CompletedTask);
        
        await mockNanoAgent.Object.Terminate();
        
        mockNanoAgent.Verify(x => x.Terminate(It.IsAny<NanoTerminationParameters>()), Times.Once);
        mockNanoAgent.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Execute_WhenInvoked_ShouldCallAppropriateMethodWithAppropriateParameters()
    {
        var now = DateTime.UtcNow;
        var mockTeraAgent = new Mock<ITeraAgent>();

        mockTeraAgent
            .Setup(x => x.Execute(
                               It.IsAny<TeraAgentExecutionParameters>()))
            .Returns(Task.CompletedTask);
        
        await mockTeraAgent.Object.Execute();
        
        mockTeraAgent.Verify(x => x.Execute(It.Is<TeraAgentExecutionParameters>(p => 
            p.ExecutionCount == 1 &&
            p.AbsoluteTimestamp >= now &&
            p.RelativeTimestamp >= now)), Times.Once);
        mockTeraAgent.VerifyNoOtherCalls();
    }
}
