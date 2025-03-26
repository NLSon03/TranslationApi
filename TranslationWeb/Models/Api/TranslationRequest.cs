using System.Text.Json.Serialization;

namespace TranslationWeb.Models.Api
{
    public class TranslationRequest
    {
        [JsonPropertyName("sourceText")]
        public string SourceText { get; set; } = string.Empty;

        [JsonPropertyName("sourceLanguage")]
        public string SourceLanguage { get; set; } = string.Empty;

        [JsonPropertyName("targetLanguage")]
        public string TargetLanguage { get; set; } = string.Empty;

        [JsonPropertyName("customTerms")]
        public List<string>? CustomTerms { get; set; }
    }
}
