namespace TranslationWeb.Core.Constants
{
    public static class ApiEndpoints
    {
        public static string BaseUrl { get; } = "http://localhost:5292";

        public static class Translation
        {
            public static string Base => $"{BaseUrl}/api/Translation";
            public static string Languages => $"{Base}/languages";
            public static string Translate => Base;
        }

        public static class Auth
        {
            public static string Base => $"{BaseUrl}/api/Auth";
            public static string Login => $"{Base}/login";
            public static string Register => $"{Base}/register";
            public static string CurrentUser => $"{Base}/current-user";
        }

        public static class Feedback
        {
            public static string Base => $"{BaseUrl}/api/Feedback";
            public static string All => Base;
            public static string ById(int id) => $"{Base}/{id}";
        }

        public static class AIModel
        {
            public static string Base => $"{BaseUrl}/api/AIModel";
            public static string All => Base;
            public static string Active => $"{Base}/active";
            public static string ById(Guid id) => $"{Base}/{id}";
            public static string Activate(Guid id) => $"{Base}/{id}/activate";
            public static string Deactivate(Guid id) => $"{Base}/{id}/deactivate";
        }

        public static class ChatSession
        {
            public static string Base => $"{BaseUrl}/api/ChatSession";
            public static string All => Base;
            public static string ById(Guid id) => $"{Base}/{id}";
            public static string ByUser => $"{Base}/user";
            public static string ByModel(Guid modelId) => $"{Base}/model/{modelId}";
            public static string Active => $"{Base}/active";
            public static string End(Guid id) => $"{Base}/{id}/end";
        }

        public static class ChatMessage
        {
            public static string Base => $"{BaseUrl}/api/ChatMessage";
            public static string BySession(Guid sessionId) => $"{Base}/session/{sessionId}";
            public static string ById(Guid id) => $"{Base}/{id}";
            public static string Send => Base;
            public static string AddFeedback => $"{Base}/feedback";
        }
    }
}