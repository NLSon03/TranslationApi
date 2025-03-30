using TranslationApi.Domain.Entities;

namespace TranslationApi.Domain.Interfaces
{
    public interface IAIModelRepository : IRepository<AIModel>
    {
        Task<IEnumerable<AIModel>> GetActiveModelsAsync();
        Task<AIModel?> GetByNameAndVersionAsync(string name, string version);
        Task DeactivateModelAsync(Guid id);
        Task<bool> ExistsAsync(string name, string version);
    }
}