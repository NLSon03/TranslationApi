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
            public static string CurrentUser => $"{Base}/me";
        }

        public static class Feedback
        {
            public static string Base => $"{BaseUrl}/api/Feedback";
            public static string All => Base;
            public static string ById(int id) => $"{Base}/{id}";
        }
    }
} 