namespace TranslationWeb.Models.AIModel
{
    public class AIModelResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Config { get; set; } = string.Empty;
        public int SessionCount { get; set; }
    }
}