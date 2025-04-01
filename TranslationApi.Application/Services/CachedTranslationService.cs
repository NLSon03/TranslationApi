
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using TranslationApi.Application.Contracts;
using TranslationApi.Application.Interfaces;

namespace TranslationApi.Application.Services
{
    public class CachedTranslationService : ITranslationService
    {
        private readonly ITranslationService _translationService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachedTranslationService> _logger;
        private const string LANGUAGES_CACHE_KEY = "supported_languages";
        private static readonly TimeSpan TRANSLATION_CACHE_DURATION = TimeSpan.FromHours(24);
        private static readonly TimeSpan LANGUAGES_CACHE_DURATION = TimeSpan.FromDays(7);

        public CachedTranslationService(
            ITranslationService translationService,
            ICacheService cacheService,
            ILogger<CachedTranslationService> logger)
        {
            _translationService = translationService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TranslationResponse> TranslateTextAsync(TranslationRequest request)
        {
            var cacheKey = GenerateTranslationCacheKey(request);
            
            try
            {
                // Try get from cache
                var cachedResult = await _cacheService.GetAsync<TranslationResponse>(cacheKey);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit for translation request. Key: {Key}", cacheKey);
                    return cachedResult;
                }

                // Get fresh translation
                _logger.LogInformation("Cache miss for translation request. Key: {Key}", cacheKey);
                var result = await _translationService.TranslateTextAsync(request);
                
                // Only cache successful translations
                if (result.Success)
                {
                    await _cacheService.SetAsync(cacheKey, result, TRANSLATION_CACHE_DURATION);
                    _logger.LogInformation("Cached translation result. Key: {Key}", cacheKey);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cached translation service");
                // Fallback to direct translation service
                return await _translationService.TranslateTextAsync(request);
            }
        }

        public async Task<IEnumerable<Language>> GetSupportedLanguagesAsync()
        {
            try
            {
                // Try get from cache
                var cachedLanguages = await _cacheService.GetAsync<IEnumerable<Language>>(LANGUAGES_CACHE_KEY);
                if (cachedLanguages != null && cachedLanguages.Any())
                {
                    _logger.LogInformation("Cache hit for supported languages");
                    return cachedLanguages;
                }

                // Get fresh languages list
                _logger.LogInformation("Cache miss for supported languages");
                var languages = await _translationService.GetSupportedLanguagesAsync();
                
                if (languages.Any())
                {
                    await _cacheService.SetAsync(LANGUAGES_CACHE_KEY, languages, LANGUAGES_CACHE_DURATION);
                    _logger.LogInformation("Cached supported languages list");
                }
                
                return languages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cached language service");
                // Fallback to direct service
                return await _translationService.GetSupportedLanguagesAsync();
            }
        }

        private string GenerateTranslationCacheKey(TranslationRequest request)
        {
            // Generate a hash of the source text to avoid long cache keys
            using var sha256 = SHA256.Create();
            var textBytes = Encoding.UTF8.GetBytes(request.SourceText);
            var hashBytes = sha256.ComputeHash(textBytes);
            var hash = Convert.ToBase64String(hashBytes);

            return $"translation:{request.SourceLanguage}:{request.TargetLanguage}:{hash}";
        }

        public async Task<bool> InvalidateTranslationCache(TranslationRequest request)
        {
            try
            {
                var cacheKey = GenerateTranslationCacheKey(request);
                await _cacheService.RemoveAsync(cacheKey);
                _logger.LogInformation("Invalidated translation cache. Key: {Key}", cacheKey);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating translation cache");
                return false;
            }
        }

        public async Task<bool> InvalidateLanguagesCache()
        {
            try
            {
                await _cacheService.RemoveAsync(LANGUAGES_CACHE_KEY);
                _logger.LogInformation("Invalidated languages cache");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating languages cache");
                return false;
            }
        }
    }
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}