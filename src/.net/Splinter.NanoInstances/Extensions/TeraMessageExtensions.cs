using System.Text.Json;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Messaging;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of extension methods for a TeraMessage instance.
/// </summary>
public static class TeraMessageExtensions
{
    /// <summary>
    /// Serializes an object to a JSON message and sets the Message property.
    /// </summary>
    public static string JsonMessage(this TeraMessage message, object? value)
    {
        var strValue = value == null
            ? string.Empty
            : JsonSerializer.Serialize(value, JsonConstants.DefaultOptions);

        message.Message = strValue;

        return strValue;
    }

    /// <summary>
    /// Deserializes the Message property as a JSON message to a Type.
    /// </summary>
    public static TValue? JsonMessage<TValue>(this TeraMessage message) where TValue : class
    {
        return message.Message.IsNullOrEmpty()
            ? null 
            : JsonSerializer.Deserialize<TValue>(message.Message, JsonConstants.DefaultOptions);
    }
}