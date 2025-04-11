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

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(request.SourceText))
            {
                return new TranslationResponse
                {
                    Success = false,
                    ErrorMessage = "Văn bản nguồn không được để trống",
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguage = request.TargetLanguage
                };
            }

            // Kiểm tra ngôn ngữ nguồn và đích không giống nhau
            if (request.SourceLanguage != "auto" && request.SourceLanguage == request.TargetLanguage)
            {
                // Nếu ngôn ngữ nguồn và đích giống nhau, trả về luôn văn bản gốc
                return new TranslationResponse
                {
                    TranslatedText = request.SourceText,
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguage = request.TargetLanguage,
                    Success = true
                };
            }

            try
            {
                var result = await TranslateWithGeminiAsync(request);

                // Lưu thông tin về ngôn ngữ nguồn được phát hiện (nếu có)
                if (request.SourceLanguage == "auto" && result.SourceLanguage == "auto")
                {
                    // Cập nhật để phản ánh ngôn ngữ thực tế được phát hiện
                    var detectedLanguage = await DetectLanguageAsync(request.SourceText);
                    result.SourceLanguage = detectedLanguage;
                }

                return result;
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
                // Tự động phát hiện ngôn ngữ nếu được yêu cầu
                var sourceLanguage = request.SourceLanguage;
                if (sourceLanguage == "auto")
                {
                    sourceLanguage = await DetectLanguageAsync(request.SourceText);
                    _logger.LogInformation($"Detected source language: {sourceLanguage}");
                }

                var textChunks = SplitLongText(request.SourceText);
                var translatedChunks = new List<string>();

                foreach (var chunk in textChunks)
                {
                    string prompt = $"Translate the following text from {GetLanguageName(sourceLanguage)} to {GetLanguageName(request.TargetLanguage)}. " +
                                    $"Follow these guidelines: " +
                                    $"1. Provide a natural and accurate translation that preserves the tone, formality, and nuances of the original text. " +
                                    $"2. Maintain all formatting, paragraph breaks, and bullet points from the original. " +
                                    $"3. Preserve proper names, brands, and technical terms unless their translation is well-established. " +
                                    $"4. For text in {GetLanguageName(request.TargetLanguage)}, make sure it sounds natural to native speakers. " +
                                    $"5. Keep emojis and special characters unchanged. " +
                                    $"6. Return ONLY the translated text without explanations or notes. " +
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
                    SourceLanguage = sourceLanguage,
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

        private async Task<string> DetectLanguageAsync(string text)
        {
            try
            {
                string prompt = $"Analyze the following text and respond with ONLY THE ISO 639-1 language code (e.g., 'en' for English, 'vi' for Vietnamese, etc.) of the language in which the text is written. No explanation, just the code.\n\nText: {text.Substring(0, Math.Min(500, text.Length))}";

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
                                    text = prompt
                                }
                            }
                        }
                    }
                };

                var content = new StringContent(
                    Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                string apiUrl = $"{_geminiApiUrl}?key={_geminiApiKey}";

                var response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var detectedLanguage = ExtractDetectedLanguage(responseBody);

                // Xác thực mã ngôn ngữ được trả về
                if (!_supportedLanguages.Any(l => l.Code.Equals(detectedLanguage, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogWarning($"Detected language '{detectedLanguage}' not in supported languages list. Defaulting to 'en'");
                    return "en";
                }

                return detectedLanguage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detecting language. Defaulting to English.");
                return "en";
            }
        }

        private string ExtractDetectedLanguage(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);

                if (!doc.RootElement.TryGetProperty("candidates", out var candidates) ||
                    candidates.GetArrayLength() == 0 ||
                    !candidates[0].TryGetProperty("content", out var content) ||
                    !content.TryGetProperty("parts", out var parts) ||
                    parts.GetArrayLength() == 0)
                {
                    _logger.LogWarning("Invalid response format for language detection");
                    return "en";
                }

                var languageCode = parts[0].GetProperty("text").GetString()?.Trim().ToLower();

                // Làm sạch mã ngôn ngữ (có thể có dấu ngoặc, dấu chấm, v.v.)
                if (!string.IsNullOrEmpty(languageCode))
                {
                    // Lấy chỉ 2 ký tự đầu tiên nếu có nhiều hơn 2 ký tự
                    var cleanedCode = new string(languageCode.Where(char.IsLetterOrDigit).Take(2).ToArray());
                    return string.IsNullOrEmpty(cleanedCode) ? "en" : cleanedCode;
                }

                return "en";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting detected language");
                return "en";
            }
        }

    }
}
