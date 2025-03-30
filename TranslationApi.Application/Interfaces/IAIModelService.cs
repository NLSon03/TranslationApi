using TranslationApi.Application.DTOs;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Interfaces
{
    public interface IAIModelService
    {
        // DTO based methods
        Task<IEnumerable<AIModelListDto>> GetAllModelDtosAsync();
        Task<IEnumerable<AIModelListDto>> GetActiveModelDtosAsync();
        Task<AIModelDetailDto?> GetModelDtoByIdAsync(Guid id);
        Task<AIModelDetailDto> CreateModelAsync(AIModelCreateDto createDto);
        Task<bool> UpdateModelAsync(Guid id, AIModelUpdateDto updateDto);

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