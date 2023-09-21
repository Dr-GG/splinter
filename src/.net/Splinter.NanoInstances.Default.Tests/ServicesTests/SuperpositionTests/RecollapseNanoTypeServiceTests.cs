using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Services.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.SuperpositionTests;

[TestFixture]
public class RecollapseNanoTypeServiceTests
{
    private const string DefaultJson = "{\"nanoTypeId\":\"604627A0-8106-43E7-9DA6-AB965FCB1199\"}";
    
    [Test]
    public async Task Recollapse_WhenIsNanoTypeAndNoParent_ReturnsZero()
    {
        var service = GetService();
        var nanoAgent = GetMockNanoAgent(hasTeraParent: false);
        var message = GetMessage();
        var result = await service.Recollapse(nanoAgent, message);

        result.Should().Be(0);
    }

    [Test]
    public async Task Recollapse_WhenNoNanoTable_ReturnsZero()
    {
        var service = GetService();
        var nanoAgent = GetMockNanoAgent(hasNanoTable: false);
        var message = GetMessage();
        var result = await service.Recollapse(nanoAgent, message);

        result.Should().Be(0);
    }

    [Test]
    public async Task Recollapse_WhenMessageIsEmpty_ReturnsZero()
    {
        var service = GetService();
        var nanoAgent = GetMockNanoAgent();
        var message = GetMessage("");
        var result = await service.Recollapse(nanoAgent, message);

        result.Should().Be(0);
    }

    [Test]
    public async Task Recollapse_WhenNanoTableHasReferencesWithNoReferences_ReturnsZero()
    {
        var service = GetService();
        var nanoAgent = GetMockNanoAgent(nanoReferences: new [] { false, false, false });
        var message = GetMessage();
        var result = await service.Recollapse(nanoAgent, message);

        result.Should().Be(0);
    }

    [Test]
    public async Task Recollapse_WhenNanoTableHasReferencesWithEmptyReferences_ReturnsZero()
    {
        var service = GetService();
        var nanoAgent = GetMockNanoAgent();
        var message = GetMessage();
        var result = await service.Recollapse(nanoAgent, message);

        result.Should().Be(0);
    }

    [Test]
    public async Task Recollapse_WhenAllDataIsValid_ReturnsTheNumberOfReferences()
    {
        var service = GetService();
        var nanoAgent = GetMockNanoAgent(nanoReferences: new[] { true, true, true });
        var message = GetMessage();
        var result = await service.Recollapse(nanoAgent, message);

        result.Should().Be(3);
    }

    [Test]
    public async Task Recollapse_WhenAllDataIsValidWithInvalidData_ReturnsTheNumberOfReferences()
    {
        var service = GetService();
        var nanoAgent = GetMockNanoAgent(nanoReferences: new[] { true, false, true, false, true });
        var message = GetMessage();
        var result = await service.Recollapse(nanoAgent, message);

        result.Should().Be(3);
    }

    private static TeraMessage GetMessage(string json = DefaultJson)
    {
        return new TeraMessage
        {
            Message = json
        };
    }

    private static INanoAgent GetMockNanoAgent(
        HolonType holonType = HolonType.Nano,
        bool hasTeraParent = true,
        bool hasNanoTable = true,
        params bool[] nanoReferences
    )
    {
        var result = new Mock<INanoAgent>();
        var mockReferences = nanoReferences.Select(GetNanoReference);
        var mockNanoTable = GetMockNanoTable(mockReferences);
        var mockTeraParent = new Mock<ITeraAgent>();
        var mockScope = new Mock<IServiceScope>();

        mockScope.Setup(s => s.Start()).ReturnsAsync(mockScope.Object);
        mockTeraParent.Setup(t => t.Scope).Returns(mockScope.Object);

        result.Setup(m => m.HolonType).Returns(holonType);
        result.Setup(m => m.HasTeraParent).Returns(hasTeraParent);
        result.Setup(m => m.HasNoTeraParent).Returns(!hasTeraParent);
        result.Setup(m => m.HasNanoTable).Returns(hasNanoTable);
        result.Setup(m => m.HasNoNanoTable).Returns(!hasNanoTable);
        result.Setup(m => m.NanoTable).Returns(mockNanoTable);
        result.Setup(m => m.TeraParent).Returns(mockTeraParent.Object);

        return result.Object;
    }

    private static INanoTable GetMockNanoTable(IEnumerable<INanoReference> references)
    {
        var result = new Mock<INanoTable>();

        result
            .Setup(r => r.Fetch(It.IsAny<Guid>()))
            .ReturnsAsync(references);

        return result.Object;
    }

    private static INanoReference GetNanoReference(bool hasReference)
    {
        var result = new Mock<INanoReference>();
        var mockReference = new Mock<INanoAgent>();
        var mockTeraParent = new Mock<ITeraAgent>();
        var mockScope = new Mock<IServiceScope>();

        mockScope.Setup(s => s.Start()).ReturnsAsync(mockScope.Object);
        mockTeraParent.Setup(t => t.Scope).Returns(mockScope.Object);
        mockReference.Setup(r => r.TeraParent).Returns(mockTeraParent.Object);

        result.Setup(r => r.HasReference).Returns(hasReference);
        result.Setup(r => r.HasNoReference).Returns(!hasReference);
        result.Setup(r => r.Reference).Returns(mockReference.Object);

        return result.Object;
    }

    private static IRecollapseNanoTypeService GetService()
    {
        return new RecollapseNanoTypeService();
    }
}