using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TranslationApi.Application.Common
{
    public static class JsonOptions
    {
        public static readonly JsonSerializerOptions SafeOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true,
            MaxDepth = 64
        };
    }
}
