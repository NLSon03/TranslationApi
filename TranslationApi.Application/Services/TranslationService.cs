
using Microsoft.Extensions.Logging;
using TranslationApi.Application.Contracts;
using TranslationApi.Application.Factories;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly ILogger<TranslationService> _logger;
        private readonly IAIModelService _aiModelService;
        private readonly IAIProviderFactory _providerFactory;

        public TranslationService(
            ILogger<TranslationService> logger,
            IAIModelService aiModelService,
            IAIProviderFactory providerFactory)
        {
            _logger = logger;
            _aiModelService = aiModelService;
            _providerFactory = providerFactory;
        }

        public async Task<TranslationResponse> TranslateTextAsync(TranslationRequest request)
        {
            try
            {
                // Get active translation model
                var activeModels = await _aiModelService.GetActiveModelDtosAsync();
                var translationModel = activeModels
                    .FirstOrDefault(m => m.ModelType.Equals("TRANSLATION", StringComparison.OrdinalIgnoreCase));

                if (translationModel == null)
                {
                    throw new InvalidOperationException("No active translation model found");
                }

                // Get the model entity
                var model = await _aiModelService.GetModelByIdAsync(translationModel.Id);
                if (model == null)
                {
                    throw new InvalidOperationException("Translation model not found");
                }

                // Create provider and translate
                var provider = _providerFactory.CreateTranslationProvider(model);
                return await provider.TranslateAsync(request);
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

        public async Task<IEnumerable<Language>> GetSupportedLanguagesAsync()
        {
            try
            {
                // Get active translation model
                var activeModels = await _aiModelService.GetActiveModelDtosAsync();
                var translationModel = activeModels
                    .FirstOrDefault(m => m.ModelType.Equals("TRANSLATION", StringComparison.OrdinalIgnoreCase));

                if (translationModel == null)
                {
                    throw new InvalidOperationException("No active translation model found");
                }

                // Get the model entity
                var model = await _aiModelService.GetModelByIdAsync(translationModel.Id);
                if (model == null)
                {
                    throw new InvalidOperationException("Translation model not found");
                }

                // Create provider and get supported languages
                var provider = _providerFactory.CreateTranslationProvider(model);
                return await provider.GetSupportedLanguagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supported languages");
                return Enumerable.Empty<Language>();
            }
        }
    }
}