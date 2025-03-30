using System.Net.Http.Json;
using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Interfaces;
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
                var response = await _httpService.GetAsync<AIModelApiResponse<IEnumerable<AIModelResponse>>>(ApiEndpoints.AIModel.Base);
                
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
                return await _httpService.GetAsync<AIModelResponse>(ApiEndpoints.AIModel.ById(id))
                    ?? new AIModelResponse { Success = false, Message = "Model not found" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AI model with ID: {Id}", id);
                return new AIModelResponse { Success = false, Message = "Failed to retrieve AI model" };
            }
        }

        public async Task<AIModelResponse> CreateModelAsync(CreateAIModelRequest request)
        {
            try
            {
                var response = await _httpService.PostAsync<CreateAIModelRequest, AIModelResponse>(
                    ApiEndpoints.AIModel.Base,
                    request
                );

                if (response != null)
                {
                    return response;
                }

                return new AIModelResponse { Success = false, Message = "Failed to create AI model" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating AI model: {ModelName}", request.Name);
                return new AIModelResponse { Success = false, Message = "Failed to create AI model" };
            }
        }

        public async Task<AIModelResponse> UpdateModelAsync(UpdateAIModelRequest request)
        {
            try
            {
                var response = await _httpService.PutAsync<UpdateAIModelRequest, AIModelResponse>(
                    ApiEndpoints.AIModel.ById(request.Id),
                    request
                );

                if (response != null)
                {
                    return response;
                }

                return new AIModelResponse { Success = false, Message = "Failed to update AI model" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AI model with ID: {Id}", request.Id);
                return new AIModelResponse { Success = false, Message = "Failed to update AI model" };
            }
        }

        public async Task<AIModelResponse> ActivateModelAsync(Guid id)
        {
            try
            {
                var response = await _httpService.PostAsync<object, AIModelResponse>(
                    ApiEndpoints.AIModel.Activate(id),
                    new { }
                );

                if (response != null)
                {
                    return response;
                }

                return new AIModelResponse { Success = false, Message = "Failed to activate AI model" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating AI model with ID: {Id}", id);
                return new AIModelResponse { Success = false, Message = "Failed to activate AI model" };
            }
        }

        public async Task<AIModelResponse> DeactivateModelAsync(Guid id)
        {
            try
            {
                var response = await _httpService.PostAsync<object, AIModelResponse>(
                    ApiEndpoints.AIModel.Deactivate(id),
                    new { }
                );

                if (response != null)
                {
                    return response;
                }

                return new AIModelResponse { Success = false, Message = "Failed to deactivate AI model" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating AI model with ID: {Id}", id);
                return new AIModelResponse { Success = false, Message = "Failed to deactivate AI model" };
            }
        }

        public async Task<bool> DeleteModelAsync(Guid id)
        {
            try
            {
                var response = await _httpService.DeleteAsync<bool>(ApiEndpoints.AIModel.ById(id));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting AI model with ID: {Id}", id);
                return false;
            }
        }
    }
}