namespace TranslationWeb.Models.AIModel
{
    public class AIModelListResponse : ApiResponse<IEnumerable<AIModelResponse>>
    {
        public IEnumerable<AIModelResponse> Models => Data ?? new List<AIModelResponse>();
    }
}