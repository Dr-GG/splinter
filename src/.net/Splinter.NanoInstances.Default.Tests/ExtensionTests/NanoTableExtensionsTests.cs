using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Extensions;
using Splinter.NanoInstances.Services.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Default.Tests.ExtensionTests;
public class NanoTableExtensionsTests
{
    [Test]
    public async Task Register_WhenInvoked_InvokesTheCorrectRegisterMethod()
    {
        var mockNanoTable = GetMockNanoTable();
        var references = GetNanoReferences().ToArray();

        await mockNanoTable.Object.Register(references);

        foreach (var reference in references)
        {
            mockNanoTable.Verify(n => n.Register(reference), Times.Once);
        }

        mockNanoTable.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Deregister_WhenInvoked_InvokesTheCorrectRegisterMethod()
    {
        var mockNanoTable = GetMockNanoTable();
        var references = GetNanoReferences().ToArray();

        await mockNanoTable.Object.Deregister(references);

        foreach (var reference in references)
        {
            mockNanoTable.Verify(n => n.Deregister(reference), Times.Once);
        }

        mockNanoTable.VerifyNoOtherCalls();
    }

    private static Mock<INanoTable> GetMockNanoTable()
    {
        return new Mock<INanoTable>();
    }

    private static IEnumerable<INanoReference> GetNanoReferences()
    {
        var result = new List<INanoReference>();

        for (var i = 0; i < 10; i++)
        {
            result.Add(new NanoReference());
        }

        return result;
    }
}
