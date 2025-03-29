namespace TranslationWeb.Models.AIModel
{
    public class AIModelListResponse
    {
        public List<AIModelResponse> Models { get; set; } = new List<AIModelResponse>();
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
    }
}