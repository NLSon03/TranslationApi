using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;

namespace TranslationApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AIModelController : ControllerBase
    {
        private readonly IAIModelService _modelService;

        public AIModelController(IAIModelService modelService)
        {
            _modelService = modelService;
        }

        /// <summary>
        /// Gets all AI models
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetAllModels()
        {
            try
            {
                var models = await _modelService.GetAllModelDtosAsync();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách models thành công",
                    data = models
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy danh sách models: " + ex.Message,
                    data = new List<AIModelListDto>()
                });
            }
        }

        /// <summary>
        /// Gets all active AI models
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AIModelListDto>>> GetActiveModels()
        {
            var models = await _modelService.GetActiveModelDtosAsync();
            return Ok(models);
        }

        /// <summary>
        /// Gets an AI model by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AIModelDetailDto>> GetModelById(Guid id)
        {
            var model = await _modelService.GetModelDtoByIdAsync(id);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        /// <summary>
        /// Creates a new AI model
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AIModelDetailDto>> CreateModel(AIModelCreateDto createDto)
        {
            if (await _modelService.ModelExistsAsync(createDto.Name, createDto.Version))
                return Conflict("Một model với tên và phiên bản này đã tồn tại");

            var result = await _modelService.CreateModelAsync(createDto);
            return CreatedAtAction(nameof(GetModelById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing AI model
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateModel(Guid id, AIModelUpdateDto updateDto)
        {
            var success = await _modelService.UpdateModelAsync(id, updateDto);

            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Activates an AI model
        /// </summary>
        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateModel(Guid id)
        {
            var model = await _modelService.GetModelDtoByIdAsync(id);

            if (model == null)
                return NotFound();

            await _modelService.ActivateModelAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Deactivates an AI model
        /// </summary>
        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateModel(Guid id)
        {
            var model = await _modelService.GetModelDtoByIdAsync(id);

            if (model == null)
                return NotFound();

            await _modelService.DeactivateModelAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Deletes an AI model
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteModel(Guid id)
        {
            var result = await _modelService.DeleteModelAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}