using System.Net.Http.Json;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.Feedback;

namespace TranslationWeb.Core.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/feedbacks";

        public FeedbackService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<FeedbackResponse>()
                        ?? new FeedbackResponse();
                }

                // Handle error response
                return new FeedbackResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new FeedbackResponse();
            }
        }

        public async Task<FeedbackResponse> UpdateFeedbackAsync(UpdateFeedbackRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{request.FeedbackId}", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<FeedbackResponse>()
                        ?? new FeedbackResponse();
                }

                // Handle error response
                return new FeedbackResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new FeedbackResponse();
            }
        }

        public async Task<FeedbackResponse> GetFeedbackByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<FeedbackResponse>($"{_baseUrl}/{id}")
                    ?? new FeedbackResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new FeedbackResponse();
            }
        }

        public async Task<FeedbackResponse> GetFeedbackByMessageIdAsync(Guid messageId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<FeedbackResponse>($"{_baseUrl}/message/{messageId}")
                    ?? new FeedbackResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new FeedbackResponse();
            }
        }

        public async Task<IEnumerable<FeedbackResponse>> GetAllFeedbacksAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<FeedbackResponse>>(_baseUrl)
                    ?? new List<FeedbackResponse>();
            }
            catch (Exception)
            {
                // Log exception
                return new List<FeedbackResponse>();
            }
        }

        public async Task<FeedbackStatsResponse> GetFeedbackStatsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<FeedbackStatsResponse>($"{_baseUrl}/stats")
                    ?? new FeedbackStatsResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new FeedbackStatsResponse();
            }
        }

        public async Task<bool> DeleteFeedbackAsync(Guid id)
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