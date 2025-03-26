using Microsoft.AspNetCore.Mvc;
using TranslationApi.Application.Interfaces;
using TranslationApi.Application.Contracts;

namespace TranslationApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly ILogger<TranslationController> _logger;
        private readonly ITranslationService _translationService;
        public TranslationController(ILogger<TranslationController> logger, ITranslationService translationService)
        {
            _logger = logger;
            _translationService = translationService;
        }
        [HttpPost]
        public async Task<ActionResult<TranslationResponse>> Translate([FromBody] TranslationRequest request)
        {
            try
            {
                _logger.LogInformation($"Translation request received: {request.SourceLanguage} to {request.TargetLanguage}");

                var result = await _translationService.TranslateTextAsync(request);

                var response = new TranslationResponse
                {
                    TranslatedText = result.TranslatedText,
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguage = request.TargetLanguage,
                    Success = true,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during translation");
                return StatusCode(500, new TranslationResponse
                {
                    Success = false,
                    ErrorMessage = "An error occurred during translation"
                });
            }
        }

        [HttpGet("languages")]
        public ActionResult<IEnumerable<Language>> GetSupportedLanguages()
        {
            var languages = new List<Language>
            {
                new Language { Code = "en", Name = "English" },
                new Language { Code = "vi", Name = "Vietnamese" },
                new Language { Code = "zh", Name = "Chinese" },
                new Language { Code = "ja", Name = "Japanese" }
            };

            return Ok(languages);
        }
    }
}
