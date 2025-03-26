namespace TranslationApi.Application.Contracts
{
    public class TranslationRequest
    {
        public string SourceText { get; set; } = string.Empty;
        public string SourceLanguage { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;
    }
}
