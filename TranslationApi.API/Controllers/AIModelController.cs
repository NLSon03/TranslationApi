using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AIModel>>> GetAllModels()
        {
            var models = await _modelService.GetAllModelsAsync();
            return Ok(models);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AIModel>>> GetActiveModels()
        {
            var models = await _modelService.GetActiveModelsAsync();
            return Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AIModel>> GetModelById(Guid id)
        {
            var model = await _modelService.GetModelByIdAsync(id);
            
            if (model == null)
                return NotFound();
                
            return Ok(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AIModel>> CreateModel(AIModel model)
        {
            if (await _modelService.ModelExistsAsync(model.Name, model.Version))
                return Conflict("Một model với tên và phiên bản này đã tồn tại");
                
            var result = await _modelService.CreateModelAsync(model);
            return CreatedAtAction(nameof(GetModelById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateModel(Guid id, AIModel model)
        {
            var existingModel = await _modelService.GetModelByIdAsync(id);
            
            if (existingModel == null)
                return NotFound();
                
            var updatedModel = new AIModel
            {
                Id = id,
                Name = model.Name,
                Version = model.Version,
                IsActive = model.IsActive,
                Config = model.Config,
                ChatSessions = existingModel.ChatSessions
            };
            
            await _modelService.UpdateModelAsync(updatedModel);
            
            return NoContent();
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateModel(Guid id)
        {
            var model = await _modelService.GetModelByIdAsync(id);
            
            if (model == null)
                return NotFound();
                
            await _modelService.ActivateModelAsync(id);
            
            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateModel(Guid id)
        {
            var model = await _modelService.GetModelByIdAsync(id);
            
            if (model == null)
                return NotFound();
                
            await _modelService.DeactivateModelAsync(id);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteModel(Guid id)
        {
            var model = await _modelService.GetModelByIdAsync(id);
            
            if (model == null)
                return NotFound();
                
            var result = await _modelService.DeleteModelAsync(id);
            
            if (result)
                return NoContent();
            else
                return BadRequest("Không thể xóa model này");
        }
    }
} 