using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<UserListResponse>> GetUsersAsync();
        Task<UserListResponse> GetUserAsync(string id);
        Task UpdateUserAsync(string id, UpdateUserRequest request);
        Task ResetPasswordAsync(string id, ChangePasswordRequest request);
        Task ToggleLockoutAsync(string id, ToggleLockoutRequest request);
    }
}