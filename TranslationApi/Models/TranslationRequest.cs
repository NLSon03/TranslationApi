namespace TranslationApi.Models
{
    public class TranslationRequest
    {
        public string SourceText { get; set; } = string.Empty;
        public string SourceLanguage { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;
        public List<String>? CustomTerms { get; set; }
    }
}
