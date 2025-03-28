using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;
using TranslationApi.Domain.Interfaces;

namespace TranslationApi.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Feedback> AddFeedbackAsync(Guid messageId, FeedbackRating rating, string comment = "")
        {
            var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
            if (message == null)
            {
                throw new ArgumentException("Message not found", nameof(messageId));
            }

            var feedback = new Feedback
            {
                MessageId = messageId,
                Rating = rating,
                Comment = comment
            };

            await _unitOfWork.Feedbacks.AddAsync(feedback);
            await _unitOfWork.CompleteAsync();
            return feedback;
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(Guid id)
        {
            return await _unitOfWork.Feedbacks.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByMessageIdAsync(Guid messageId)
        {
            return await _unitOfWork.Feedbacks.GetFeedbacksByMessageIdAsync(messageId);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksBySessionIdAsync(Guid sessionId)
        {
            return await _unitOfWork.Feedbacks.GetFeedbacksBySessionIdAsync(sessionId);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByRatingAsync(FeedbackRating rating)
        {
            return await _unitOfWork.Feedbacks.GetFeedbacksByRatingAsync(rating);
        }

        public async Task<double> GetAverageRatingAsync(Guid sessionId)
        {
            return await _unitOfWork.Feedbacks.GetAverageRatingAsync(sessionId);
        }

        public async Task<Dictionary<FeedbackRating, int>> GetRatingDistributionAsync(Guid sessionId)
        {
            return await _unitOfWork.Feedbacks.GetRatingDistributionAsync(sessionId);
        }

        public async Task UpdateFeedbackAsync(Guid id, FeedbackRating rating, string comment)
        {
            var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
            if (feedback != null)
            {
                feedback.Rating = rating;
                feedback.Comment = comment;
                
                await _unitOfWork.Feedbacks.UpdateAsync(feedback);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteFeedbackAsync(Guid id)
        {
            var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
            if (feedback != null)
            {
                await _unitOfWork.Feedbacks.RemoveAsync(feedback);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
} 