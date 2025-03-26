using System.Text.Json.Serialization;

namespace TranslationWeb.Models.Api
{
    public class Language
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
