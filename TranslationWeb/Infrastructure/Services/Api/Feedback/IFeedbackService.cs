using TranslationWeb.Models.Feedback;

namespace TranslationWeb.Infrastructure.Services.Api.Feedback
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackResponse>> GetAllFeedbacksAsync();
        Task<FeedbackResponse> GetFeedbackByIdAsync(int id);
        Task<FeedbackResponse> AddFeedbackAsync(FeedbackRequest feedbackRequest);
        Task<bool> DeleteFeedbackAsync(int id);
    }
} 