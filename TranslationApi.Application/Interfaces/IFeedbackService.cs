using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;

namespace TranslationApi.Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<Feedback> AddFeedbackAsync(Guid messageId, FeedbackRating rating, string comment = "");
        Task<Feedback?> GetFeedbackByIdAsync(Guid id);
        Task<IEnumerable<Feedback>> GetFeedbacksByMessageIdAsync(Guid messageId);
        Task<IEnumerable<Feedback>> GetFeedbacksBySessionIdAsync(Guid sessionId);
        Task<IEnumerable<Feedback>> GetFeedbacksByRatingAsync(FeedbackRating rating);
        Task<double> GetAverageRatingAsync(Guid sessionId);
        Task<Dictionary<FeedbackRating, int>> GetRatingDistributionAsync(Guid sessionId);
        Task UpdateFeedbackAsync(Guid id, FeedbackRating rating, string comment);
        Task DeleteFeedbackAsync(Guid id);
    }
}