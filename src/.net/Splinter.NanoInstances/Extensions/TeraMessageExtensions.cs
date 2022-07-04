using System.Text.Json;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Messaging;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Extensions;

public static class TeraMessageExtensions
{
    public static string JsonMessage(this TeraMessage message, object? value)
    {
        var strValue = value == null
            ? string.Empty
            : JsonSerializer.Serialize(value, JsonConstants.DefaultOptions);

        message.Message = strValue;

        return strValue;
    }

    public static TValue? JsonMessage<TValue>(this TeraMessage message) where TValue : class
    {
        return message.Message.IsNullOrEmpty()
            ? null 
            : JsonSerializer.Deserialize<TValue>(message.Message, JsonConstants.DefaultOptions);
    }
}