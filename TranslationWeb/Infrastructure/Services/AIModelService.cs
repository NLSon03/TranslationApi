using System.Net.Http.Json;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.AIModel;

namespace TranslationWeb.Infrastructure.Services
{
    public class AIModelService : IAIModelService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/aimodels";

        public AIModelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AIModelListResponse> GetAllModelsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<AIModelListResponse>(_baseUrl);
                return response ?? new AIModelListResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new AIModelListResponse();
            }
        }

        public async Task<AIModelResponse> GetModelByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<AIModelResponse>($"{_baseUrl}/{id}")
                    ?? new AIModelResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new AIModelResponse();
            }
        }

        public async Task<AIModelResponse> CreateModelAsync(CreateAIModelRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AIModelResponse>()
                        ?? new AIModelResponse();
                }

                // Handle error response
                return new AIModelResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new AIModelResponse();
            }
        }

        public async Task<AIModelResponse> UpdateModelAsync(UpdateAIModelRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{request.Id}", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AIModelResponse>()
                        ?? new AIModelResponse();
                }

                // Handle error response
                return new AIModelResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new AIModelResponse();
            }
        }

        public async Task<AIModelResponse> ActivateModelAsync(Guid id)
        {
            try
            {
                var request = new ActivateModelRequest { Id = id };
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/activate", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AIModelResponse>()
                        ?? new AIModelResponse();
                }

                // Handle error response
                return new AIModelResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new AIModelResponse();
            }
        }

        public async Task<AIModelResponse> DeactivateModelAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/{id}/deactivate", null);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AIModelResponse>()
                        ?? new AIModelResponse();
                }

                // Handle error response
                return new AIModelResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new AIModelResponse();
            }
        }

        public async Task<bool> DeleteModelAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }
    }
}