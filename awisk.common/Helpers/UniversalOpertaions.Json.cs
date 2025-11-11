using System.Text.Json;
using System.Text.Json.Serialization;

namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static readonly JsonSerializerOptions DefaultJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public static string ToJson<T>(this T obj, JsonSerializerOptions? opt = null)
            => JsonSerializer.Serialize(obj, opt ?? DefaultJsonOptions);

        public static T? FromJson<T>(this string? json, JsonSerializerOptions? opt = null)
            => string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json!, opt ?? DefaultJsonOptions);
    }
}
