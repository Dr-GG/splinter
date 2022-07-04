using System.Threading.Tasks;
using NUnit.Framework;
using Splinter.NanoInstances.Services.Superposition;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;

namespace Splinter.NanoInstances.Tests.ServicesTests.SuperpositionTests;

[TestFixture]
public class NanoReferenceTests
{
    [Test]
    public void HasNoReference_WhenCreatingEmptyReference_ReturnsTrue()
    {
        var reference = new NanoReference();

        Assert.IsTrue(reference.HasNoReference);
        Assert.IsFalse(reference.HasReference);
    }

    [Test]
    public void Reference_WhenCreatingEmptyReference_ThrowsException()
    {
        var reference = new NanoReference();

        Assert.Throws<InvalidNanoInstanceException>(() =>
        {
            _ = reference.Reference;
        });
    }

    [Test]
    public async Task HasNoReference_WhenSettingNullReference_ReturnsTrue()
    {
        var reference = new NanoReference();

        await reference.Initialise(null);

        Assert.IsTrue(reference.HasNoReference);
        Assert.IsFalse(reference.HasReference);
    }

    [Test]
    public async Task Reference_WhenSettingNullReference_ThrowsException()
    {
        var reference = new NanoReference();

        await reference.Initialise(null);

        Assert.Throws<InvalidNanoInstanceException>(() =>
        {
            _ = reference.Reference;
        });
    }

    [Test]
    public async Task HasNoReference_WhenSettingReference_ReturnsFalse()
    {
        var agent = new UnitTestNanoAgent();
        var reference = new NanoReference();

        await reference.Initialise(agent);

        Assert.IsFalse(reference.HasNoReference);
        Assert.IsTrue(reference.HasReference);
    }

    [Test]
    public async Task Reference_WhenSettingReference_ThrowsNoException()
    {
        var agent = new UnitTestNanoAgent();
        var reference = new NanoReference();

        await reference.Initialise(agent);

        Assert.AreEqual(agent, reference.Reference);
    }
}