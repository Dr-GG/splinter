using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Services.ServiceScope;
using Splinter.NanoTypes.Domain.Exceptions.ServiceScope;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoInstances.Tests.ServicesTests.ServiceScopeTests;

public class AutofacServiceScopeTests
{
    [Test]
    public void Constructor_WhenInvoked_CreatesANewInstance()
    {
        var mockLifetimeScope = GetMockDefaultLifetimeScope();
        
        GetDefaultServiceScope(mockLifetimeScope.Object);

        mockLifetimeScope.Verify(x => x.BeginLifetimeScope(), Times.Once);
    }

    [Test]
    public async Task Start_WhenInvoked_ReturnsANewInstance()
    {
        var scope = GetDefaultServiceScope();
        var result = await scope.Start();

        result.Should().NotBeNull();
        result.Should().NotBeSameAs(scope);
    }

    [Test]
    public void StartSync_WhenInvoked_ReturnsANewInstance()
    {
        var scope = GetDefaultServiceScope();
        var result = scope.StartSync();

        result.Should().NotBeNull();
        result.Should().NotBeSameAs(scope);
    }

    [Test]
    public void ResolveSync_WhenInvokedAndNoServiceIsResolved_ThrowsAnException()
    {
        var lifetimeScope = GetRealAutofacLifetimeScope();
        var scope = GetDefaultServiceScope(lifetimeScope);
        var error = Assert.Throws<ServiceUnresolvedException>(() => scope.ResolveSync<ITeraAgent>())!;

        error.Should().NotBeNull();
        error.Message.Should().Be($"Could not resolve the service type {typeof(ITeraAgent).FullName}.");
    }

    [Test]
    public void Resolve_WhenInvokedAndNoServiceIsResolved_ThrowsAnException()
    {
        var lifetimeScope = GetRealAutofacLifetimeScope();
        var scope = GetDefaultServiceScope(lifetimeScope);
        var error = Assert.ThrowsAsync<ServiceUnresolvedException>(() => scope.Resolve<ITeraAgent>())!;

        error.Should().NotBeNull();
        error.Message.Should().Be($"Could not resolve the service type {typeof(ITeraAgent).FullName}.");
    }

    [Test]
    public void ResolveSync_WhenInvokedAndServiceIsResolved_ReturnsResolvedService()
    {
        var lifetimeScope = GetRealAutofacLifetimeScope();
        var scope = GetDefaultServiceScope(lifetimeScope);
        var nanoTable = scope.ResolveSync<INanoTable>();

        nanoTable.Should().NotBeNull();
    }

    [Test]
    public async Task Resolve_WhenInvokedAndServiceIsResolved_ReturnsResolvedService()
    {
        var lifetimeScope = GetRealAutofacLifetimeScope();
        var scope = GetDefaultServiceScope(lifetimeScope);
        var nanoTable = await scope.Resolve<INanoTable>();

        nanoTable.Should().NotBeNull();
    }

    [Test]
    public async Task DisposeAsync_WhenInvoked_CallsTheAppropriateMethods()
    {
        var mockLifetimeScope = GetMockDefaultLifetimeScope();
        var scope = GetDefaultServiceScope(mockLifetimeScope.Object);

        await scope.DisposeAsync();

        mockLifetimeScope.Verify(x => x.Dispose(), Times.Once);
    }

    [Test]
    public void Dispose_WhenInvoked_CallsTheAppropriateMethods()
    {
        var mockLifetimeScope = GetMockDefaultLifetimeScope();
        var scope = GetDefaultServiceScope(mockLifetimeScope.Object);

        scope.Dispose();

        mockLifetimeScope.Verify(x => x.Dispose(), Times.Once);
    }

    private static ILifetimeScope GetRealAutofacLifetimeScope()
    {
        var builder = new ContainerBuilder();

        builder
            .Register(c => new Mock<INanoTable>().Object)
            .As<INanoTable>()
            .InstancePerDependency();

        return builder.Build();
    }

    private static Mock<ILifetimeScope> GetMockDefaultLifetimeScope()
    {
        var result = new Mock<ILifetimeScope>();

        result
            .Setup(x => x.BeginLifetimeScope())
            .Returns(result.Object);

        return result;
    }

    private static ILifetimeScope GetDefaultLifetimeScope()
    {
        return GetMockDefaultLifetimeScope().Object;
    }

    private static IServiceScope GetDefaultServiceScope(ILifetimeScope? scope = null)
    {
        scope ??= GetDefaultLifetimeScope();

        return new AutofacServiceScope(scope);
    }
}
