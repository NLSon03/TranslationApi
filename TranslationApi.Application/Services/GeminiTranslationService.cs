using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using TranslationApi.Application.Contracts;
using TranslationApi.Application.Interfaces;

namespace TranslationApi.Application.Services
{
    public class GeminiTranslationService : ITranslationService
    {
        private readonly ILogger<GeminiTranslationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiKey;
        private readonly string _geminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        private static readonly JsonSerializerOptions SafeOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        private readonly List<Language> _supportedLanguages = new List<Language>{
            new Language { Code = "en", Name = "English" },
            new Language { Code = "vi", Name = "Vietnamese" },
            new Language { Code = "zh", Name = "Chinese" },
            new Language { Code = "ja", Name = "Japanese" },
            new Language { Code = "ko", Name = "Korean" },
            new Language { Code = "fr", Name = "French" },
            new Language { Code = "de", Name = "German" },
            new Language { Code = "es", Name = "Spanish" },
            new Language { Code = "ru", Name = "Russian" },
            new Language { Code = "ar", Name = "Arabic" },
            new Language { Code = "hi", Name = "Hindi" },
            new Language { Code = "id", Name = "Indonesian" },
            new Language { Code = "it", Name = "Italian" },
            new Language { Code = "nl", Name = "Dutch" },
            new Language { Code = "pl", Name = "Polish" },
            new Language { Code = "pt", Name = "Portuguese" },
            new Language { Code = "th", Name = "Thai" },
            new Language { Code = "tr", Name = "Turkish" },
            new Language { Code = "uk", Name = "Ukrainian" }
        };

        public GeminiTranslationService(ILogger<GeminiTranslationService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _geminiApiKey = _configuration["GeminiApi:ApiKey"] ?? "AIzaSyB4ZJM1Mu8m6-ypPs_bajwh2UmzpKfi3PM";
        }

        public async Task<IEnumerable<Language>> GetSupportedLanguagesAsync()
        {
            return await Task.FromResult(_supportedLanguages);
        }

        public async Task<TranslationResponse> TranslateTextAsync(TranslationRequest request)
        {
            _logger.LogInformation($"Translating text from {request.SourceLanguage} to {request.TargetLanguage}");
            try
            {
                return await TranslateWithGeminiAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during translation");
                return new TranslationResponse
                {
                    Success = false,
                    ErrorMessage = $"Translation failed: {ex.Message}",
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguage = request.TargetLanguage
                };
            }
        }
        private async Task<TranslationResponse> TranslateWithGeminiAsync(TranslationRequest request)
        {
            try
            {
                var textChunks = SplitLongText(request.SourceText);

                var translatedChunks = new List<string>();

                foreach (var chunk in textChunks)
                {
                    string prompt = $"Translate the following text from {GetLanguageName(request.SourceLanguage)} to {GetLanguageName(request.TargetLanguage)}. " +
                                    $"Provide only the most literal translation possible, avoiding any paraphrasing or invented content. " +
                                    $"Text to translate: {chunk}";

                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new
                            {
                                parts = new[]
                                {
                                    new
                                    {
                                        text =prompt
                                    }
                                }
                            }
                        }
                    };
                    _logger.LogDebug($"Request Payload: {Serialize(requestBody)}");
                    var content = new StringContent(
                        Serialize(requestBody),
                         Encoding.UTF8,
                         "application/json"
                        );
                    string apiUrl = $"{_geminiApiUrl}?key={_geminiApiKey}";

                    var response = await _httpClient.PostAsync(apiUrl, content);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var translatedPart = ExtractTranslatedText(responseBody);

                    translatedChunks.Add(translatedPart);
                }

                return new TranslationResponse
                {
                    TranslatedText = string.Join(" ", translatedChunks),
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguage = request.TargetLanguage,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error using Gemini API for translation");
                throw;
            }
        }

        private string GetLanguageName(string languageCode)
        {
            var language = _supportedLanguages.FirstOrDefault(l => l.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
            return language?.Name ?? languageCode;
        }

        public string NormalizeText(string sourceText)
        {
            if (string.IsNullOrEmpty(sourceText))
                return string.Empty;

            string normalizedText = sourceText.Normalize(NormalizationForm.FormKC);

            normalizedText = Regex.Replace(normalizedText, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");
            normalizedText = Regex.Replace(normalizedText, @"[\r\n\t]+", " ");
            normalizedText = Regex.Replace(normalizedText, @"\s+", " ").Trim();
            return normalizedText;
        }

        public List<string> SplitLongText(string sourceText)
        {
            int maxLength = 300000;

            string normalizedText = NormalizeText(sourceText);
            normalizedText = SanitizeForJson(normalizedText);
            var chunks = new List<string>();

            if (string.IsNullOrEmpty(normalizedText))
                return chunks;

            if (normalizedText.Length <= maxLength)
            {
                chunks.Add(normalizedText);
                return chunks;
            }

            for (int i = 0; i < normalizedText.Length; i++)
            {
                int length = Math.Min(maxLength, normalizedText.Length - i);
                int endIndex = i + length;

                while (endIndex > i && !char.IsWhiteSpace(normalizedText[endIndex - 1]))
                {
                    endIndex--;
                }

                if (endIndex <= i)
                {
                    endIndex = i + length;
                }
                chunks.Add(normalizedText.Substring(i, endIndex - i).Trim());
                i = endIndex - 1;
            }
            return chunks;
        }


        public static string Serialize(object obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, SafeOptions);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"JSON Serialization Error: {ex.Message}");
                throw;
            }
        }

        public static T Deserialize<T>(string json)
        {
            try
            {
                var result = JsonSerializer.Deserialize<T>(json, SafeOptions);
                if (result == null)
                {
                    throw new NullReferenceException("Deserialization returned null.");
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"JSON Deserialization Error: {ex.Message}");
                throw;
            }
        }

        private string SanitizeForJson(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] controlChars = {
        '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005',
        '\u0006', '\u0007', '\u0008', '\u000B', '\u000C', '\u000E',
        '\u000F', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014',
        '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001A',
        '\u001B', '\u001C', '\u001D', '\u001E', '\u001F'
    };

            var sanitized = new string(input
                .Where(c => !controlChars.Contains(c))
                .ToArray());

            sanitized = Regex.Replace(sanitized, @"[\r\n]+", " ");

            sanitized = sanitized
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\t", "\\t")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Trim();

            return sanitized;
        }
        private string ExtractTranslatedText(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);

                // Log response for debugging
                _logger.LogDebug($"Full API Response: {responseBody}");

                // Validate response structure
                if (!doc.RootElement.TryGetProperty("candidates", out var candidates))
                {
                    throw new Exception("Response missing 'candidates' property");
                }

                if (candidates.GetArrayLength() == 0)
                {
                    throw new Exception("No translation candidates found");
                }

                var candidate = candidates[0];
                if (!candidate.TryGetProperty("content", out var content))
                {
                    throw new Exception("Response missing 'content' property");
                }

                if (!content.TryGetProperty("parts", out var parts))
                {
                    throw new Exception("Response missing 'parts' property");
                }

                if (parts.GetArrayLength() == 0)
                {
                    throw new Exception("No content parts found");
                }

                var translatedText = parts[0].GetProperty("text").GetString();
                if (string.IsNullOrEmpty(translatedText))
                {
                    throw new Exception("Translated text is empty");
                }

                return translatedText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting translated text from response: {ResponseBody}", responseBody);
                throw new Exception($"Failed to extract translation: {ex.Message}");
            }
        }

        private async Task<string> TranslateWithRetryAsync(string apiUrl, StringContent content)
        {
            int maxRetries = 3;
            int currentRetry = 0;
            int baseDelay = 1000; // 1 second

            while (currentRetry < maxRetries)
            {
                try
                {
                    var response = await _httpClient.PostAsync(apiUrl, content);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    currentRetry++;
                    if (currentRetry >= maxRetries)
                    {
                        _logger.LogError(ex, "Failed to translate after {Retries} retries", maxRetries);
                        throw;
                    }

                    var delay = baseDelay * Math.Pow(2, currentRetry - 1); // Exponential backoff
                    _logger.LogWarning(ex, "Translation attempt {Attempt} failed, retrying in {Delay}ms", currentRetry, delay);
                    await Task.Delay((int)delay);
                }
            }

            throw new Exception("Unexpected error in retry logic");
        }

    }
}
