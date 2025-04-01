
namespace TranslationApi.Application.Providers.Implementations
{
    public static class PromptTemplates
    {
        public static string GetTranslationPrompt(string sourceLang, string targetLang, string sourceText, string textType = "general")
        {
            var basePrompt = $@"You are a professional translator with expertise in {sourceLang} and {targetLang}.

Task: Translate the following text from {sourceLang} to {targetLang}.

Translation Requirements:
1. Maintain the original meaning and context with high accuracy
2. Preserve all formatting, including paragraphs, lists, and special characters
3. Keep the same tone and style (formal/informal) as the source text
4. Use standard translations for technical terms
5. For idioms and cultural references, use appropriate cultural equivalents
6. Maintain any HTML or Markdown formatting if present

Source Text:
{sourceText}

Important Guidelines:
- Provide ONLY the translation without explanations or notes
- Preserve all paragraph breaks and text structure
- Keep numbers, dates, and proper nouns unchanged unless translation is necessary
- Match the source text's formality level
- Ensure natural flow in the target language";

            // Add specific instructions based on text type
            var additionalInstructions = textType switch
            {
                "technical" => @"
Additional Technical Guidelines:
- Maintain technical terminology consistency
- Preserve code snippets and technical symbols exactly as they appear
- Use industry-standard translations for technical terms
- Keep mathematical formulas and equations unchanged",

                "formal" => @"
Additional Formal Guidelines:
- Use formal language and honorifics appropriate for the target language
- Maintain professional tone throughout
- Use standard business terminology
- Preserve formal document structure",

                "creative" => @"
Additional Creative Guidelines:
- Preserve creative elements and wordplay where possible
- Adapt cultural references to target language equivalents
- Maintain rhythm and flow in creative writing
- Preserve emotional impact and style",

                "legal" => @"
Additional Legal Guidelines:
- Use precise legal terminology appropriate for the target jurisdiction
- Maintain exact meaning without interpretation
- Preserve legal document formatting
- Keep any legal references in their official form",

                _ => string.Empty // No additional instructions for general text
            };

            return basePrompt + additionalInstructions + "\n\nTranslation:";
        }

        public static Dictionary<string, object> GetPromptConfig(string textType)
        {
            // Default configuration
            var baseConfig = new Dictionary<string, object>
            {
                { "temperature", 0.3 },
                { "topP", 0.8 },
                { "maxTokens", 1000 },
                { "stopSequences", new[] { "Source Text:", "Important Guidelines:" } }
            };

            // Adjust configuration based on text type
            switch (textType)
            {
                case "technical":
                    baseConfig["temperature"] = 0.2; // More precise
                    baseConfig["topP"] = 0.9;
                    break;

                case "creative":
                    baseConfig["temperature"] = 0.7; // More creative
                    baseConfig["topP"] = 0.95;
                    break;

                case "legal":
                    baseConfig["temperature"] = 0.1; // Most precise
                    baseConfig["topP"] = 0.8;
                    break;

                case "formal":
                    baseConfig["temperature"] = 0.25; // Precise but natural
                    baseConfig["topP"] = 0.85;
                    break;
            }

            return baseConfig;
        }

        public static string DetectTextType(string text)
        {
            // Simple heuristics to detect text type
            if (text.Contains("<code>") || text.Contains("```") || 
                text.Contains("function") || text.Contains("class "))
            {
                return "technical";
            }

            if (text.Contains("Dear Sir/Madam") || text.Contains("Sincerely,") ||
                text.Contains("Best regards") || text.Contains("To whom it may concern"))
            {
                return "formal";
            }

            if (text.Contains("WHEREAS") || text.Contains("hereinafter") ||
                text.Contains("pursuant to") || text.Contains("IN WITNESS WHEREOF"))
            {
                return "legal";
            }

            if (text.Contains("!") || text.Contains("?") || text.Length < 100 ||
                text.Contains("..."))
            {
                return "creative";
            }

            return "general";
        }
    }
}