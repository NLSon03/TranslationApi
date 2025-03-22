using Microsoft.AspNetCore.Mvc;

namespace TranslationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly ILogger<TranslationController> _logger;

        public TranslationController(ILogger<TranslationController> logger)
        {
            _logger = logger;
        }

    }
}
