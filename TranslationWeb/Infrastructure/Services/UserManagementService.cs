
using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly HttpService _httpClient;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            HttpService httpClient,
            ILogger<UserManagementService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<UserListResponse>> GetUsersAsync()
        {
            try
            {
                _logger.LogInformation("Đang lấy danh sách người dùng");
                var response = await _httpClient.GetAsync<List<UserListResponse>>(ApiEndpoints.Auth.Users.GetAll);
                return response ?? new List<UserListResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách người dùng");
                throw new Exception("Không thể tải danh sách người dùng", ex);
            }
        }

        public async Task<UserListResponse> GetUserAsync(string id)
        {
            try
            {
                _logger.LogInformation("Đang lấy thông tin người dùng: {UserId}", id);
                var response = await _httpClient.GetAsync<UserListResponse>(ApiEndpoints.Auth.Users.GetById(id));
                if (response == null)
                {
                    throw new Exception("Không thể tải thông tin người dùng");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng: {UserId}", id);
                throw new Exception("Không thể tải thông tin người dùng", ex);
            }
        }

        public async Task UpdateUserAsync(string id, UpdateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Đang cập nhật thông tin người dùng: {UserId}", id);
                await _httpClient.PutAsync<UpdateUserRequest, object>(ApiEndpoints.Auth.Users.Update(id), request);
                _logger.LogInformation("Cập nhật thông tin người dùng thành công: {UserId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin người dùng: {UserId}", id);
                throw new Exception("Không thể cập nhật thông tin người dùng", ex);
            }
        }

        public async Task ResetPasswordAsync(string id, ChangePasswordRequest request)
        {
            try
            {
                _logger.LogInformation("Đang đặt lại mật khẩu cho người dùng: {UserId}", id);
                await _httpClient.PostAsync<ChangePasswordRequest, object>(ApiEndpoints.Auth.Users.ResetPassword(id), request);
                _logger.LogInformation("Đặt lại mật khẩu thành công cho người dùng: {UserId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu cho người dùng: {UserId}", id);
                throw new Exception("Không thể đặt lại mật khẩu", ex);
            }
        }

        public async Task ToggleLockoutAsync(string id, ToggleLockoutRequest request)
        {
            try
            {
                _logger.LogInformation("Đang thay đổi trạng thái khóa cho người dùng: {UserId}, IsLocked: {IsLocked}", id, request.IsLocked);
                await _httpClient.PostAsync<ToggleLockoutRequest, object>(ApiEndpoints.Auth.Users.ToggleLockout(id), request);
                _logger.LogInformation("Thay đổi trạng thái khóa thành công cho người dùng: {UserId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thay đổi trạng thái khóa cho người dùng: {UserId}", id);
                throw new Exception("Không thể thay đổi trạng thái khóa tài khoản", ex);
            }
        }
    }
}