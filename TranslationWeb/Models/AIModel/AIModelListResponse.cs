namespace TranslationWeb.Models.AIModel
{
    public class AIModelListResponse : AIModelApiResponse<IEnumerable<AIModelResponse>>
    {
        public IEnumerable<AIModelResponse> Models => Data ?? new List<AIModelResponse>();
    }
}