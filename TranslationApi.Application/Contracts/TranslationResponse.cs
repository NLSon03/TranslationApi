namespace TranslationApi.Application.Contracts
{
    public class TranslationResponse
    {
        public string TranslatedText { get; set; } = string.Empty;
        public string SourceLanguage { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
