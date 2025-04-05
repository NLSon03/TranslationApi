using TranslationWeb.Models.AIModel;

namespace TranslationWeb.Infrastructure.Interfaces
{
    public interface IAIModelService
    {
        Task<AIModelListResponse> GetAllModelsAsync();
        Task<AIModelResponse> GetModelByIdAsync(Guid id);
        Task<AIModelResponse> CreateModelAsync(AIModelRequest request);
        Task<AIModelResponse> UpdateModelAsync(UpdateAIModelRequest request);
        Task<AIModelResponse> ActivateModelAsync(Guid id);
        Task<AIModelResponse> DeactivateModelAsync(Guid id);
        Task<AIModelResponse> DeleteModelAsync(Guid id);
    }
}