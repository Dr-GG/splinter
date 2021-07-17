using System.Text.Json;

namespace Splinter.NanoTypes.Domain.Constants
{
    public static class JsonConstants
    {
        public static readonly JsonSerializerOptions DefaultOptions = new()
        {
            AllowTrailingCommas = true,
            IgnoreNullValues = true,
            IgnoreReadOnlyFields = true,
            PropertyNameCaseInsensitive = false,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
