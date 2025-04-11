using Microsoft.AspNetCore.Http;

namespace TranslationApi.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string userId, string fileType);
        Task DeleteFileAsync(string fileUrl);
        Task<bool> IsValidImageAsync(IFormFile file);
    }
}