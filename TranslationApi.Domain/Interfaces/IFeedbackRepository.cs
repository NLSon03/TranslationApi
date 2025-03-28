using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;

namespace TranslationApi.Domain.Interfaces
{
    public interface IFeedbackRepository : IRepository<Feedback>
    {
        Task<IEnumerable<Feedback>> GetFeedbacksByMessageIdAsync(Guid messageId);
        Task<IEnumerable<Feedback>> GetFeedbacksBySessionIdAsync(Guid sessionId);
        Task<IEnumerable<Feedback>> GetFeedbacksByRatingAsync(FeedbackRating rating);
        Task<double> GetAverageRatingAsync(Guid sessionId);
        Task<Dictionary<FeedbackRating, int>> GetRatingDistributionAsync(Guid sessionId);
    }
} 