using TranslationApi.Application.DTOs;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Interfaces
{
    public interface IAIModelService
    {
        // DTO based methods
        Task<IEnumerable<AIModelDto>> GetAllModelDtosAsync();
        Task<IEnumerable<AIModelDto>> GetActiveModelDtosAsync();
        Task<AIModelDto> GetModelDtoByIdAsync(Guid id);
        Task<AIModelDto> CreateModelAsync(AIModelDto createDto);
        Task<bool> UpdateModelAsync(Guid id, AIModelDto updateDto);

        // Existing methods for internal use
        Task<bool> ModelExistsAsync(string name, string version);
        Task ActivateModelAsync(Guid id);
        Task DeactivateModelAsync(Guid id);
        Task<bool> DeleteModelAsync(Guid id);

        // Keep these methods internal for service layer use
        Task<AIModel?> GetModelByIdAsync(Guid id);
        Task<AIModel?> GetModelByNameAndVersionAsync(string name, string version);
    }
}