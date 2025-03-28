using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;
using TranslationApi.Domain.Interfaces;
using TranslationApi.Infrastructure.Data;

namespace TranslationApi.Infrastructure.Repositories
{
    public class FeedbackRepository : Repository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByMessageIdAsync(Guid messageId)
        {
            return await _dbSet
                .Where(f => f.MessageId == messageId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksBySessionIdAsync(Guid sessionId)
        {
            return await _dbSet
                .Include(f => f.Message)
                .Where(f => f.Message.SessionId == sessionId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByRatingAsync(FeedbackRating rating)
        {
            return await _dbSet
                .Where(f => f.Rating == rating)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(Guid sessionId)
        {
            var feedbacks = await GetFeedbacksBySessionIdAsync(sessionId);
            if (!feedbacks.Any())
                return 0;

            return feedbacks.Average(f => (int)f.Rating);
        }

        public async Task<Dictionary<FeedbackRating, int>> GetRatingDistributionAsync(Guid sessionId)
        {
            var feedbacks = await GetFeedbacksBySessionIdAsync(sessionId);
            
            var distribution = new Dictionary<FeedbackRating, int>();
            foreach (FeedbackRating rating in Enum.GetValues(typeof(FeedbackRating)))
            {
                distribution[rating] = feedbacks.Count(f => f.Rating == rating);
            }

            return distribution;
        }
    }
} 
 