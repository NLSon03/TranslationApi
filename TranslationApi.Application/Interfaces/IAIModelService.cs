using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Interfaces
{
    public interface IAIModelService
    {
        Task<AIModel> CreateModelAsync(AIModel model);
        Task<AIModel?> GetModelByIdAsync(Guid id);
        Task<IEnumerable<AIModel>> GetAllModelsAsync();
        Task<IEnumerable<AIModel>> GetActiveModelsAsync();
        Task<AIModel?> GetModelByNameAndVersionAsync(string name, string version);
        Task UpdateModelAsync(AIModel model);
        Task ActivateModelAsync(Guid id);
        Task DeactivateModelAsync(Guid id);
        Task<bool> DeleteModelAsync(Guid id);
        Task<bool> ModelExistsAsync(string name, string version);
    }
} 