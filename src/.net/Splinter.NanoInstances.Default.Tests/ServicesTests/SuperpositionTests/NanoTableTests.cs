using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Extensions;
using Splinter.NanoInstances.Default.Services.Superposition;
using Splinter.NanoInstances.Default.Tests.Agents.NanoAgents;
using Splinter.NanoInstances.Services.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Superposition;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.SuperpositionTests;

[TestFixture]
public class NanoTableTests
{
    private static readonly Guid TestNanoTypeId1 = new("{8E75CE7E-040D-452D-BA95-595CE649C6EA}");
    private static readonly Guid TestNanoTypeId2 = new("{0575A396-F40B-48BB-8110-22C573DC21D6}");
    private static readonly Guid TestNanoTypeId3 = new("{2C45DA19-56CA-44AC-96C0-03844E61B8CD}");

    private const int NumberOfThreads = 10000;

    [Test]
    public void Register_WhenProvidingNullReferences_ExecutesSuccessfully()
    {
        var table = new NanoTable();

        Assert.DoesNotThrowAsync(async () =>
        {
            await table.Register(GetNullableNanoReference());
            await table.Register(GetNullableNanoReference());
            await table.Register(GetNullableNanoReference());
            await table.Register(GetNullableNanoReference());
            await table.Register(GetNullableNanoReference());
        });
    }

    [Test]
    public async Task Register_WhenProvidingReferences_RegistersAllReferencesCorrectly()
    {
        var table = new NanoTable();
        var ref11 = GetNanoReference1();
        var ref12 = GetNanoReference1();
        var ref13 = GetNanoReference1();
        var ref14 = GetNanoReference1();
        var ref15 = GetNanoReference1();
        var ref21 = GetNanoReference2();
        var ref22 = GetNanoReference2();
        var ref23 = GetNanoReference2();
        var ref24 = GetNanoReference2();
        var ref31 = GetNanoReference3();
        var ref32 = GetNanoReference3();
        var ref33 = GetNanoReference3();

        await table.Register(
            ref11, ref12, ref13, ref14, ref15,
            ref21, ref22, ref23, ref24,
            ref31, ref32, ref33);

        await AssertNonNullableReferences(table, TestNanoTypeId1, 5, ref11, ref12, ref13, ref14, ref15);
        await AssertNonNullableReferences(table, TestNanoTypeId2, 4, ref21, ref22, ref23, ref24);
        await AssertNonNullableReferences(table, TestNanoTypeId3, 3, ref31, ref32, ref33);
    }

    [Test]
    public async Task Register_WhenDisposingOfReferences_RegistersAllReferencesCorrectly()
    {
        var table = new NanoTable();
        var ref11 = GetNanoReference1();
        var ref12 = GetNanoReference1();
        var ref13 = GetNanoReference1();
        var ref14 = GetNanoReference1();
        var ref15 = GetNanoReference1();
        var ref21 = GetNanoReference2();
        var ref22 = GetNanoReference2();
        var ref23 = GetNanoReference2();
        var ref24 = GetNanoReference2();
        var ref31 = GetNanoReference3();
        var ref32 = GetNanoReference3();
        var ref33 = GetNanoReference3();

        await table.Register(
            ref11, ref12, ref13, ref14, ref15,
            ref21, ref22, ref23, ref24,
            ref31, ref32, ref33);

        await table.Dispose(ref13, ref21, ref33);

        await AssertNonNullableReferences(table, TestNanoTypeId1, 4, ref11, ref12, ref14, ref15);
        await AssertNonNullableReferences(table, TestNanoTypeId2, 3, ref22, ref23, ref24);
        await AssertNonNullableReferences(table, TestNanoTypeId3, 2, ref31, ref32);
    }

    [Test]
    public async Task Register_WhenRegisteringAReferenceAndSettingItAsNull_DoesNotReturnTheReference()
    {
        var table = new NanoTable();
        var reference = GetNanoReference1();

        await table.Register(reference);
        await reference.Initialise(null);

        await AssertNonNullableReferences(table, TestNanoTypeId1, 0);
    }

    [Test]
    public async Task Register_WhenMultipleThreadsRegisterAReference_TheSingleReferenceExists()
    {
        var table = new NanoTable();
        var ref1 = GetNanoReference1();
        var ref2 = GetNanoReference2();
        var ref3 = GetNanoReference3();
        var result = new List<Task>();

        for (var i = 0; i < NumberOfThreads; i++)
        {
            result.Add(Task.Run(() => table.Register(ref1)));
            result.Add(Task.Run(() => table.Register(ref2)));
            result.Add(Task.Run(() => table.Register(ref3)));
        }

        Task.WaitAll(result.ToArray());

        await AssertNonNullableReferences(table, TestNanoTypeId1, 1, ref1);
        await AssertNonNullableReferences(table, TestNanoTypeId2, 1, ref2);
        await AssertNonNullableReferences(table, TestNanoTypeId3, 1, ref3);
    }

    [Test]
    public async Task Register_WhenMultipleThreadsDisposeAReference_ThrowsNoErrorsAndNoReferencesExist()
    {
        var table = new NanoTable();
        var ref1 = GetNanoReference1();
        var ref2 = GetNanoReference2();
        var ref3 = GetNanoReference3();
        var result = new List<Task>();

        await table.Register(ref1, ref2, ref3);

        for (var i = 0; i < NumberOfThreads; i++)
        {
            result.Add(Task.Run(() => table.Dispose(ref1)));
            result.Add(Task.Run(() => table.Dispose(ref2)));
            result.Add(Task.Run(() => table.Dispose(ref3)));
        }

        Task.WaitAll(result.ToArray());

        await AssertNonNullableReferences(table, TestNanoTypeId1, 0);
        await AssertNonNullableReferences(table, TestNanoTypeId2, 0);
        await AssertNonNullableReferences(table, TestNanoTypeId3, 0);
    }

    private static INanoReference GetNanoReference1()
    {
        return GetNanoReference(TestNanoTypeId1);
    }

    private static INanoReference GetNanoReference2()
    {
        return GetNanoReference(TestNanoTypeId2);
    }

    private static INanoReference GetNanoReference3()
    {
        return GetNanoReference(TestNanoTypeId3);
    }

    private static INanoReference GetNullableNanoReference()
    {
        return GetNanoReference();
    }

    private static async Task AssertNonNullableReferences(
        INanoTable table,
        Guid nanoTypeId,
        int expectedCount,
        params INanoReference[] references)
    {
        var collection = (await table.Fetch(nanoTypeId)).ToList();

        collection.Count.Should().Be(expectedCount);

        if (references.IsEmpty())
        {
            return;
        }

        references.All(r => 
            collection.Any(c => 
                ((NanoTableUnitTestAgent) c.Reference)
                .IsReference(r))).Should().BeTrue();
    }

    private static INanoReference GetNanoReference(Guid? nanoTypeId = null)
    {
        if (nanoTypeId == null)
        {
            return new NanoReference();
        }

        var nanoAgent = new NanoTableUnitTestAgent(nanoTypeId.Value);
        var reference = new NanoReference();

        reference.Initialise(nanoAgent);

        return reference;
    }
}