using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models;
using TranslationWeb.Models.AIModel;

namespace TranslationWeb.Infrastructure.Services
{
    public class AIModelService : IAIModelService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<AIModelService> _logger;
        private readonly LocalStorageService _storageService;

        public AIModelService(HttpService httpService, ILogger<AIModelService> logger, LocalStorageService storageService)
        {
            _httpService = httpService;
            _logger = logger;
            _storageService = storageService;
        }

        public async Task<AIModelListResponse> GetAllModelsAsync()
        {
            try
            {
                _logger.LogInformation("Đang gọi API để lấy danh sách AI models");
                var response = await _httpService.GetAsync<AIModelListResponse>(ApiEndpoints.AIModel.Base);

                if (response == null)
                {
                    _logger.LogWarning("API trả về response null");
                    return new AIModelListResponse
                    {
                        Success = false,
                        Message = "Không nhận được phản hồi từ server",
                        Data = new List<AIModelResponse>()
                    };
                }

                var result = new AIModelListResponse
                {
                    Success = response.Success,
                    Message = response.Message,
                    Data = response.Data ?? new List<AIModelResponse>()
                };

                // Đánh dấu mỗi model là thành công nếu API trả về thành công
                if (result.Success && result.Data != null)
                {
                    foreach (var model in result.Data)
                    {
                        model.Success = true;
                    }
                }

                _logger.LogInformation("Đã nhận được {count} models từ API", result.Models.Count());

                if (!result.Success)
                {
                    _logger.LogWarning("API trả về thất bại với message: {message}", result.Message);
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Lỗi kết nối khi gọi API get all models");
                return new AIModelListResponse
                {
                    Success = false,
                    Message = "Không thể kết nối đến server. Vui lòng thử lại sau.",
                    Data = new List<AIModelResponse>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy danh sách AI models");
                return new AIModelListResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách models. Vui lòng thử lại sau.",
                    Data = new List<AIModelResponse>()
                };
            }
        }

        public async Task<AIModelResponse> GetModelByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Đang gọi API để lấy chi tiết AI model: {ModelId}", id);
                
                var response = await _httpService.GetAsync<ApiResponse<AIModelResponse>>(
                    ApiEndpoints.AIModel.ById(id)
                );

                if (response == null)
                {
                    _logger.LogWarning("API trả về response null khi lấy chi tiết model");
                    return new AIModelResponse { Success = false, Message = "Không nhận được phản hồi từ server" };
                }

                if (!response.Success)
                {
                    _logger.LogWarning("API trả về thất bại khi lấy chi tiết model: {Message}", response.Message);
                    return new AIModelResponse { Success = false, Message = response.Message ?? "Lỗi khi lấy thông tin model" };
                }

                if (response.Data == null)
                {
                    _logger.LogWarning("API trả về data null khi lấy chi tiết model");
                    return new AIModelResponse { Success = false, Message = "Không tìm thấy thông tin model" };
                }

                response.Data.Success = true;
                _logger.LogInformation("Lấy chi tiết model thành công: {ModelId}", id);
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết AI model: {ModelId}", id);
                return new AIModelResponse { Success = false, Message = "Lỗi khi lấy thông tin model: " + ex.Message };
            }
        }

        public async Task<AIModelResponse> CreateModelAsync(AIModelRequest request)
        {
            try
            {
                _logger.LogInformation("Đang gọi API để tạo AI model mới: {ModelName}", request.Name);
                
                var response = await _httpService.PostAsync<AIModelRequest, AIModelResponse>(
                    ApiEndpoints.AIModel.Base,
                    request
                );

                if (response == null)
                {
                    _logger.LogWarning("API trả về response null khi tạo model");
                    return new AIModelResponse { Success = false, Message = "Không nhận được phản hồi từ server" };
                }

                // Đánh dấu response là thành công vì API trả về model trực tiếp
                response.Success = true;
                _logger.LogInformation("Tạo model mới thành công: {ModelName}", response.Name);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo AI model: {ModelName}", request.Name);
                return new AIModelResponse { Success = false, Message = "Lỗi khi tạo model: " + ex.Message };
            }
        }

        public async Task<AIModelResponse> UpdateModelAsync(UpdateAIModelRequest request)
        {
            try
            {
                _logger.LogInformation("Đang gọi API để cập nhật AI model: {ModelId}", request.Id);
                
                var response = await _httpService.PutAsync<AIModelRequest, AIModelResponse>(
                    ApiEndpoints.AIModel.ById(request.Id),
                    request
                );

                if (response == null)
                {
                    _logger.LogWarning("API trả về response null khi cập nhật model");
                    return new AIModelResponse { Success = false, Message = "Không nhận được phản hồi từ server" };
                }

                // Đánh dấu response là thành công vì API trả về model trực tiếp
                response.Success = true;
                _logger.LogInformation("Cập nhật model thành công: {ModelId}", request.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật AI model: {ModelId}", request.Id);
                return new AIModelResponse { Success = false, Message = "Lỗi khi cập nhật model: " + ex.Message };
            }
        }

        public async Task<AIModelResponse> ActivateModelAsync(Guid id)
        {
            try
            {
                var response = await _httpService.PostAsync<object, ApiResponse<object>>(
                    ApiEndpoints.AIModel.Activate(id),
                    new { }
                );

                if (response == null)
                {
                    return new AIModelResponse 
                    { 
                        Success = false, 
                        Message = "Không nhận được phản hồi từ server" 
                    };
                }

                return new AIModelResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating AI model with ID: {Id}", id);
                return new AIModelResponse 
                { 
                    Success = false, 
                    Message = "Lỗi khi kích hoạt model: " + ex.Message 
                };
            }
        }

        public async Task<AIModelResponse> DeactivateModelAsync(Guid id)
        {
            try
            {
                var response = await _httpService.PostAsync<object, ApiResponse<object>>(
                    ApiEndpoints.AIModel.Deactivate(id),
                    new { }
                );

                if (response == null)
                {
                    return new AIModelResponse 
                    { 
                        Success = false, 
                        Message = "Không nhận được phản hồi từ server" 
                    };
                }

                return new AIModelResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating AI model with ID: {Id}", id);
                return new AIModelResponse 
                { 
                    Success = false, 
                    Message = "Lỗi khi vô hiệu hóa model: " + ex.Message 
                };
            }
        }

        public async Task<AIModelResponse> DeleteModelAsync(Guid id)
        {
            try
            {
                var response = await _httpService.DeleteAsync<ApiResponse<object>>(ApiEndpoints.AIModel.ById(id));
                
                if (response == null)
                {
                    return new AIModelResponse 
                    { 
                        Success = false, 
                        Message = "Không nhận được phản hồi từ server" 
                    };
                }

                return new AIModelResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting AI model with ID: {Id}", id);
                return new AIModelResponse 
                { 
                    Success = false, 
                    Message = "Lỗi khi xóa model: " + ex.Message 
                };
            }
        }
    }
}