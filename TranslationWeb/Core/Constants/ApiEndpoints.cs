using System;

namespace TranslationWeb.Core.Constants
{
    public static class ApiEndpoints
    {
        public static string BaseUrl { get; } = "https://localhost:5293";

        public static class Auth
        {
            public static string Base => $"{BaseUrl}/api/Auth";
            public static string Login => $"{Base}/login";
            public static string Register => $"{Base}/register";
            public static string Logout => $"{Base}/logout";
            public static string RefreshToken => $"{Base}/refresh-token";
            public static string CurrentUser => $"{Base}/current-user";
            public static string GoogleLogin => $"{Base}/google-login";
            public static string GoogleResponse => $"{Base}/google-response";

            public static class Users
            {
                private static string Base => $"{Auth.Base}/users";
                public static string GetAll => Base;
                public static string GetById(string userId) => $"{Base}/{userId}";
                public static string Update(string userId) => $"{Base}/{userId}";
                public static string ResetPassword(string userId) => $"{Base}/{userId}/reset-password";
                public static string ToggleLockout(string userId) => $"{Base}/{userId}/toggle-lockout";
                public static string ChangePassword(string userId) => $"{Base}/{userId}/change-password";
                public static string UpdateProfile(string userId) => $"{Base}/{userId}/profile";
            }
        }

        public static class Translation
        {
            public static string Base => $"{BaseUrl}/api/Translation";
            public static string Languages => $"{Base}/languages";
            public static string Translate => Base;
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

        public static class System
        {
            private static string Base => $"{BaseUrl}/api";
            public static string Health => $"{Base}/health";
        }
    }
}