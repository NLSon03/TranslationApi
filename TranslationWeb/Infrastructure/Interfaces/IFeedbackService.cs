using TranslationWeb.Models.Feedback;

namespace TranslationWeb.Infrastructure.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request);
        Task<FeedbackResponse> UpdateFeedbackAsync(UpdateFeedbackRequest request);
        Task<FeedbackResponse> GetFeedbackByIdAsync(Guid id);
        Task<FeedbackResponse> GetFeedbackByMessageIdAsync(Guid messageId);
        Task<IEnumerable<FeedbackResponse>> GetAllFeedbacksAsync();
        Task<FeedbackStatsResponse> GetFeedbackStatsAsync();
        Task<bool> DeleteFeedbackAsync(Guid id);
    }
}