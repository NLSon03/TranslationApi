
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TranslationApi.Application.Contracts;

namespace TranslationApi.Application.Providers.Implementations
{
    public class GeminiTranslationProvider : ITranslationProvider
    {
        private readonly ILogger<GeminiTranslationProvider> _logger;
        private readonly HttpClient _httpClient;
        private string _apiKey = string.Empty;
        private string _apiUrl = string.Empty;
        private List<Language> _supportedLanguages = new();
        private int _maxTextLength = 300000;

        private static readonly JsonSerializerOptions SafeOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public string ProviderName => "GEMINI";

        public GeminiTranslationProvider(
            ILogger<GeminiTranslationProvider> logger,
            HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task Initialize(string config)
        {
            try
            {
                var configObj = JsonSerializer.Deserialize<GeminiConfig>(config);
                if (configObj == null)
                    throw new ArgumentException("Invalid configuration");

                _apiKey = configObj.ApiKey;
                _apiUrl = configObj.ApiUrl;
                _supportedLanguages = configObj.SupportedLanguages;
                _maxTextLength = configObj.MaxTextLength;

                await ValidateConfig(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Gemini provider");
                throw;
            }
        }

        public async Task<bool> ValidateConfig(string config)
        {
            try
            {
                var configObj = JsonSerializer.Deserialize<GeminiConfig>(config);
                if (configObj == null)
                    return false;

                if (string.IsNullOrEmpty(configObj.ApiKey) || string.IsNullOrEmpty(configObj.ApiUrl))
                    return false;

                // Test API connection
                var testRequest = new TranslationRequest
                {
                    SourceText = "test",
                    SourceLanguage = "en",
                    TargetLanguage = "vi"
                };

                var response = await TranslateAsync(testRequest);
                return response.Success;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TranslationResponse> TranslateAsync(TranslationRequest request)
        {
            _logger.LogInformation($"Translating text from {request.SourceLanguage} to {request.TargetLanguage}");
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
                                    new { text = prompt }
                                }
                            }
                        }
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(requestBody, SafeOptions),
                        Encoding.UTF8,
                        "application/json"
                    );

                    string apiUrl = $"{_apiUrl}?key={_apiKey}";
                    var response = await _httpClient.PostAsync(apiUrl, content);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var translatedPart = ExtractTranslatedText(responseBody);
                    translatedChunks.Add(translatedPart);
                }

                return new TranslationResponse
                {
                    TranslatedText = string.Join("\n", translatedChunks),
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguage = request.TargetLanguage,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error using Gemini API for translation");
                return new TranslationResponse
                {
                    Success = false,
                    ErrorMessage = $"Translation failed: {ex.Message}",
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguage = request.TargetLanguage
                };
            }
        }

        public Task<IEnumerable<Language>> GetSupportedLanguagesAsync()
        {
            return Task.FromResult<IEnumerable<Language>>(_supportedLanguages);
        }

        private string GetLanguageName(string languageCode)
        {
            var language = _supportedLanguages.FirstOrDefault(l => 
                l.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
            return language?.Name ?? languageCode;
        }

        private List<string> SplitLongText(string sourceText)
        {
            string normalizedText = NormalizeText(sourceText);
            normalizedText = SanitizeForJson(normalizedText);
            var chunks = new List<string>();

            if (string.IsNullOrEmpty(normalizedText))
                return chunks;

            if (normalizedText.Length <= _maxTextLength)
            {
                chunks.Add(normalizedText);
                return chunks;
            }

            for (int i = 0; i < normalizedText.Length; i++)
            {
                int length = Math.Min(_maxTextLength, normalizedText.Length - i);
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

        private string NormalizeText(string sourceText)
        {
            if (string.IsNullOrEmpty(sourceText))
                return string.Empty;

            string normalizedText = sourceText.Normalize(NormalizationForm.FormKC);
            normalizedText = Regex.Replace(normalizedText, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");
            normalizedText = Regex.Replace(normalizedText, @"[\r\n\t]+", " ");
            normalizedText = Regex.Replace(normalizedText, @"\s+", " ").Trim();
            return normalizedText;
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
                _logger.LogDebug($"Full API Response: {responseBody}");

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

        private class GeminiConfig
        {
            public string ApiKey { get; set; } = string.Empty;
            public string ApiUrl { get; set; } = string.Empty;
            public List<Language> SupportedLanguages { get; set; } = new();
            public int MaxTextLength { get; set; } = 300000;
        }
    }
}