using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Interfaces;

namespace TranslationApi.Application.Services
{
    public class AIModelService : IAIModelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AIModelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<AIModel> CreateModelAsync(AIModel model)
        {
            await _unitOfWork.AIModels.AddAsync(model);
            await _unitOfWork.CompleteAsync();
            return model;
        }

        public async Task<AIModel?> GetModelByIdAsync(Guid id)
        {
            return await _unitOfWork.AIModels.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AIModel>> GetAllModelsAsync()
        {
            return await _unitOfWork.AIModels.GetAllAsync();
        }

        public async Task<IEnumerable<AIModel>> GetActiveModelsAsync()
        {
            return await _unitOfWork.AIModels.GetActiveModelsAsync();
        }

        public async Task<AIModel?> GetModelByNameAndVersionAsync(string name, string version)
        {
            return await _unitOfWork.AIModels.GetByNameAndVersionAsync(name, version);
        }

        public async Task UpdateModelAsync(AIModel model)
        {
            await _unitOfWork.AIModels.UpdateAsync(model);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ActivateModelAsync(Guid id)
        {
            var model = await _unitOfWork.AIModels.GetByIdAsync(id);
            if (model != null)
            {
                model.IsActive = true;
                await _unitOfWork.AIModels.UpdateAsync(model);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeactivateModelAsync(Guid id)
        {
            await _unitOfWork.AIModels.DeactivateModelAsync(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> DeleteModelAsync(Guid id)
        {
            var model = await _unitOfWork.AIModels.GetByIdAsync(id);
            if (model != null)
            {
                await _unitOfWork.AIModels.RemoveAsync(model);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ModelExistsAsync(string name, string version)
        {
            return await _unitOfWork.AIModels.ExistsAsync(name, version);
        }
    }
} 