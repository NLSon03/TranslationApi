namespace TranslationWeb.Models.Translation
{
    public class TranslationHistoryItem
    {
        public string SourceLanguage { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;
        public string SourceText { get; set; } = string.Empty;
        public string TranslatedText { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
