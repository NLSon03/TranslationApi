using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Interfaces;

namespace TranslationApi.Application.Services
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatSessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ChatSession> CreateSessionAsync(string userId, Guid modelId)
        {
            var model = await _unitOfWork.AIModels.GetByIdAsync(modelId);
            if (model == null)
            {
                throw new ArgumentException("Model not found", nameof(modelId));
            }

            var session = new ChatSession
            {
                UserId = userId,
                AIModelId = modelId,
                AIModel = model,
                Status = ChatSessionStatus.Active
            };

            await _unitOfWork.ChatSessions.AddAsync(session);
            await _unitOfWork.CompleteAsync();
            return session;
        }

        public async Task<ChatSession?> GetSessionByIdAsync(Guid id)
        {
            return await _unitOfWork.ChatSessions.GetByIdAsync(id);
        }

        public async Task<ChatSession?> GetSessionWithMessagesAsync(Guid id)
        {
            return await _unitOfWork.ChatSessions.GetSessionWithMessagesAsync(id);
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByUserIdAsync(string userId)
        {
            return await _unitOfWork.ChatSessions.GetSessionsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<ChatSession>> GetAllSessionsAsync()
        {
            return await _unitOfWork.ChatSessions.GetAllAsync();
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByModelIdAsync(Guid modelId)
        {
            return await _unitOfWork.ChatSessions.GetSessionsByModelIdAsync(modelId);
        }

        public async Task<IEnumerable<ChatSession>> GetActiveSessionsAsync()
        {
            return await _unitOfWork.ChatSessions.GetActiveSessions();
        }

        public async Task EndSessionAsync(Guid id)
        {
            await _unitOfWork.ChatSessions.EndSessionAsync(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteSessionAsync(Guid id)
        {
            var session = await _unitOfWork.ChatSessions.GetByIdAsync(id);
            if (session != null)
            {
                await _unitOfWork.ChatSessions.RemoveAsync(session);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> IsUserSessionOwnerAsync(Guid sessionId, string userId)
        {
            var session = await _unitOfWork.ChatSessions.GetByIdAsync(sessionId);
            return session?.UserId == userId;
        }
    }
} 