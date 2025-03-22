namespace TranslationApi.Models
{
    public class CustomTerm
    {
        public required int Id { get; set; }
        public required string SourceTerm { get; set; }
        public string TargetTerm { get; set; } = string.Empty;
        public string SourceLanguage { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;
    }
}
