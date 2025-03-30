using System.Text.Json.Serialization;

namespace TranslationWeb.Models.AIModel
{
    public class AIModelResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Config { get; set; } = string.Empty;
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class AIModelApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}