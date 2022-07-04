using System.Text.Json;
using System.Text.Json.Serialization;

namespace Splinter.NanoTypes.Domain.Constants;

public static class JsonConstants
{
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