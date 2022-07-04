using System;
using NUnit.Framework;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Tests.Models;
using Splinter.NanoTypes.Domain.Messaging;

namespace Splinter.NanoInstances.Tests.ExtensionsTests;

public class TeraMessageExtensionsTests
{
    private const string TestJson = "{\"property1\":1,\"property2\":1.22,\"property3\":2.33,\"property4\":\"property 4\",\"property5\":\"1983-10-03T00:00:00\"}";

    [Test]
    public void JsonMessage_WhenProvidingAnObject_SerialisesCorrectly()
    {
        var teraMessage = new TeraMessage();
        var data = new UnitTestTeraMessageData
        {
            Property1 = 1,
            Property2 = 1.22,
            Property3 = 2.33f,
            Property4 = "property 4",
            Property5 = new DateTime(1983, 10, 03)
        };
        var json = teraMessage.JsonMessage(data);

        Assert.AreEqual(TestJson, json);
        Assert.AreEqual(TestJson, teraMessage.Message);
    }

    [Test]
    public void JsonMessage_WhenObjectIsNull_SerialisesAnEmptyString()
    {
        var teraMessage = new TeraMessage();
        var json = teraMessage.JsonMessage(null);

        Assert.AreEqual(string.Empty, json);
        Assert.AreEqual(string.Empty, teraMessage.Message);
    }

    [TestCase("")]
    [TestCase(null)]
    public void JsonMessage_WhenProvidingAnEmptyString_ReturnsNull(string value)
    {
        var teraMessage = new TeraMessage {Message = value};
        var result = teraMessage.JsonMessage<UnitTestTeraMessageData>();

        Assert.IsNull(result);
    }
        
    [Test]
    public void JsonMessage_WhenProvidingAValidJsonString_ReturnsTheObject()
    {
        var teraMessage = new TeraMessage { Message = TestJson };
        var result = teraMessage.JsonMessage<UnitTestTeraMessageData>();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.Property1);
        Assert.AreEqual(1.22, result.Property2);
        Assert.AreEqual(2.33f, result.Property3);
        Assert.AreEqual("property 4", result.Property4);
        Assert.AreEqual(new DateTime(1983, 10, 03), result.Property5);
    }
}