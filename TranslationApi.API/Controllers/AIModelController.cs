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
                    data = new List<AIModelDto>()
                });
            }
        }

        /// <summary>
        /// Gets all active AI models
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<object>> GetActiveModels()
        {
            try
            {
                var models = await _modelService.GetActiveModelDtosAsync();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách models hoạt động thành công",
                    data = models
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy danh sách models hoạt động: " + ex.Message,
                    data = new List<AIModelDto>()
                });
            }
        }

        /// <summary>
        /// Gets an AI model by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetModelById(Guid id)
        {
            try
            {
                var model = await _modelService.GetModelDtoByIdAsync(id);

                if (model == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy model",
                        data = null as object
                    });

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin model thành công",
                    data = model
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy thông tin model: " + ex.Message,
                    data = null as object
                });
            }
        }

        /// <summary>
        /// Creates a new AI model
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AIModelDto>> CreateModel(AIModelDto createDto)
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
        public async Task<ActionResult<object>> UpdateModel(Guid id, AIModelDto updateDto)
        {
            try
            {
                var success = await _modelService.UpdateModelAsync(id, updateDto);

                if (!success)
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy model cần cập nhật",
                        data = null as object
                    });

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật model thành công",
                    data = null as object
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi cập nhật model: " + ex.Message,
                    data = null as object
                });
            }
        }

        /// <summary>
        /// Activates an AI model
        /// </summary>
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> ActivateModel(Guid id)
        {
            try
            {
                var model = await _modelService.GetModelDtoByIdAsync(id);

                if (model == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy model cần kích hoạt",
                        data = null as object
                    });

                await _modelService.ActivateModelAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Kích hoạt model thành công",
                    data = null as object
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi kích hoạt model: " + ex.Message,
                    data = null as object
                });
            }
        }

        /// <summary>
        /// Deactivates an AI model
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> DeactivateModel(Guid id)
        {
            try
            {
                var model = await _modelService.GetModelDtoByIdAsync(id);

                if (model == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy model cần vô hiệu hóa",
                        data = null as object
                    });

                await _modelService.DeactivateModelAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Vô hiệu hóa model thành công",
                    data = null as object
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi vô hiệu hóa model: " + ex.Message,
                    data = null as object
                });
            }
        }

        /// <summary>
        /// Deletes an AI model
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> DeleteModel(Guid id)
        {
            try
            {
                var result = await _modelService.DeleteModelAsync(id);

                if (!result)
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy model cần xóa",
                        data = null as object
                    });

                return Ok(new
                {
                    success = true,
                    message = "Xóa model thành công",
                    data = null as object
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi xóa model: " + ex.Message,
                    data = null as object
                });
            }
        }
    }
}