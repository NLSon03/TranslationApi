using TranslationWeb.Models.AIModel;

namespace TranslationWeb.Core.Interfaces
{
    public interface IAIModelService
    {
        Task<AIModelListResponse> GetAllModelsAsync();
        Task<AIModelResponse> GetModelByIdAsync(Guid id);
        Task<AIModelResponse> CreateModelAsync(CreateAIModelRequest request);
        Task<AIModelResponse> UpdateModelAsync(UpdateAIModelRequest request);
        Task<AIModelResponse> ActivateModelAsync(Guid id);
        Task<AIModelResponse> DeactivateModelAsync(Guid id);
        Task<bool> DeleteModelAsync(Guid id);
    }
}