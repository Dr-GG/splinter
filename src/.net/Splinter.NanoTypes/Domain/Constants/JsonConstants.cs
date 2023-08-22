using System.Text.Json;
using System.Text.Json.Serialization;

namespace Splinter.NanoTypes.Domain.Constants;

/// <summary>
/// A collection of constants used for JSON serialization/deserialization purposes in Splinter.
/// </summary>
public static class JsonConstants
{
    /// <summary>
    /// The default JsonSerializerOptions instance to be used.
    /// </summary>
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        AllowTrailingCommas = true,
        IgnoreReadOnlyFields = true,
        PropertyNameCaseInsensitive = false,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}