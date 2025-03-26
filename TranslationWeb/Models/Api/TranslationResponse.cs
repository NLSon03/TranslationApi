using System.Text.Json.Serialization;

namespace TranslationWeb.Models.Api
{
    public class TranslationResponse
    {
        [JsonPropertyName("translatedText")]
        public string TranslatedText { get; set; } = string.Empty;

        [JsonPropertyName("sourceLanguage")]
        public string SourceLanguage { get; set; } = string.Empty;

        [JsonPropertyName("targetLanguage")]
        public string TargetLanguage { get; set; } = string.Empty;

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}
