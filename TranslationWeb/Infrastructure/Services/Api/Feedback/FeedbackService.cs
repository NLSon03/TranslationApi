using Microsoft.Extensions.Logging;
using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Services.Http;
using TranslationWeb.Models.Feedback;

namespace TranslationWeb.Infrastructure.Services.Api.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(HttpService httpService, ILogger<FeedbackService> logger)
        {
            _httpService = httpService;
            _logger = logger;
        }

        public async Task<IEnumerable<FeedbackResponse>> GetAllFeedbacksAsync()
        {
            try
            {
                var response = await _httpService.GetAsync<IEnumerable<FeedbackResponse>>(ApiEndpoints.Feedback.All);
                return response ?? new List<FeedbackResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all feedbacks");
                return new List<FeedbackResponse>();
            }
        }

        public async Task<FeedbackResponse> GetFeedbackByIdAsync(int id)
        {
            try
            {
                var response = await _httpService.GetAsync<FeedbackResponse>(ApiEndpoints.Feedback.ById(id));
                return response ?? new FeedbackResponse { Success = false, Message = "Không tìm thấy phản hồi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching feedback with id {Id}", id);
                return new FeedbackResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<FeedbackResponse> AddFeedbackAsync(FeedbackRequest feedbackRequest)
        {
            try
            {
                var response = await _httpService.PostAsync<FeedbackResponse>(ApiEndpoints.Feedback.All, feedbackRequest);
                return response ?? new FeedbackResponse { Success = false, Message = "Không thể tạo phản hồi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback");
                return new FeedbackResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            try
            {
                return await _httpService.DeleteAsync(ApiEndpoints.Feedback.ById(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback with id {Id}", id);
                return false;
            }
        }
    }
} 