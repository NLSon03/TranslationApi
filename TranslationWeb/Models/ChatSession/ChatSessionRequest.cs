namespace TranslationWeb.Models.ChatSession
{
    public class CreateSessionRequest
    {
        public Guid AIModelId { get; set; }
    }

    public class EndSessionRequest
    {
        public Guid SessionId { get; set; }
    }
}