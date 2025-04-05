using TranslationApi.Application.Contracts;

namespace TranslationApi.Application.Common
{
    public static class SupportedLanguages
    {
        public static readonly List<Language> All = new List<Language>
        {
            new Language { Code = "en", Name = "English" },
            new Language { Code = "ja", Name = "Japanese" },
            new Language { Code = "ko", Name = "Korean" },
            new Language { Code = "ar", Name = "Arabic" },
            new Language { Code = "id", Name = "Bahasa Indonesia" },
            new Language { Code = "bn", Name = "Bengali" },
            new Language { Code = "bg", Name = "Bulgarian" },
            new Language { Code = "zh-Hans", Name = "Chinese (Simplified)" },
            new Language { Code = "zh-Hant", Name = "Chinese (Traditional)" },
            new Language { Code = "hr", Name = "Croatian" },
            new Language { Code = "cs", Name = "Czech" },
            new Language { Code = "da", Name = "Danish" },
            new Language { Code = "nl", Name = "Dutch" },
            new Language { Code = "et", Name = "Estonian" },
            new Language { Code = "fa", Name = "Farsi" },
            new Language { Code = "fi", Name = "Finnish" },
            new Language { Code = "fr", Name = "French" },
            new Language { Code = "de", Name = "German" },
            new Language { Code = "gu", Name = "Gujarati" },
            new Language { Code = "el", Name = "Greek" },
            new Language { Code = "he", Name = "Hebrew" },
            new Language { Code = "hi", Name = "Hindi" },
            new Language { Code = "hu", Name = "Hungarian" },
            new Language { Code = "it", Name = "Italian" },
            new Language { Code = "kn", Name = "Kannada" },
            new Language { Code = "lv", Name = "Latvian" },
            new Language { Code = "lt", Name = "Lithuanian" },
            new Language { Code = "ml", Name = "Malayalam" },
            new Language { Code = "mr", Name = "Marathi" },
            new Language { Code = "no", Name = "Norwegian" },
            new Language { Code = "pl", Name = "Polish" },
            new Language { Code = "pt", Name = "Portuguese" },
            new Language { Code = "ro", Name = "Romanian" },
            new Language { Code = "ru", Name = "Russian" },
            new Language { Code = "sr", Name = "Serbian" },
            new Language { Code = "sk", Name = "Slovak" },
            new Language { Code = "sl", Name = "Slovenian" },
            new Language { Code = "es", Name = "Spanish" },
            new Language { Code = "sw", Name = "Swahili" },
            new Language { Code = "sv", Name = "Swedish" },
            new Language { Code = "ta", Name = "Tamil" },
            new Language { Code = "te", Name = "Telugu" },
            new Language { Code = "th", Name = "Thai" },
            new Language { Code = "tr", Name = "Turkish" },
            new Language { Code = "uk", Name = "Ukrainian" },
            new Language { Code = "ur", Name = "Urdu" },
            new Language { Code = "vi", Name = "Vietnamese" }
        };
    }
}
