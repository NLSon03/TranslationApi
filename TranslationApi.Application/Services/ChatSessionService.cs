using AutoMapper;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Interfaces;

namespace TranslationApi.Application.Services
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatSessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ChatSessionDto> CreateSessionAsync(string userId, Guid modelId)
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
            return _mapper.Map<ChatSessionDto>(session);
        }

        public async Task<ChatSessionDto?> GetSessionByIdAsync(Guid id)
        {
            var session = await _unitOfWork.ChatSessions.GetByIdAsync(id);
            return session != null ? _mapper.Map<ChatSessionDto>(session) : null;
        }

        public async Task<ChatSessionDetailDto?> GetSessionWithMessagesAsync(Guid id)
        {
            var session = await _unitOfWork.ChatSessions.GetSessionWithMessagesAsync(id);
            return session != null ? _mapper.Map<ChatSessionDetailDto>(session) : null;
        }

        public async Task<IEnumerable<ChatSessionDto>> GetSessionsByUserIdAsync(string userId)
        {
            var sessions = await _unitOfWork.ChatSessions.GetSessionsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ChatSessionDto>>(sessions);
        }

        public async Task<IEnumerable<ChatSessionDto>> GetAllSessionsAsync()
        {
            var sessions = await _unitOfWork.ChatSessions.GetAllAsync();
            return _mapper.Map<IEnumerable<ChatSessionDto>>(sessions);
        }

        public async Task<IEnumerable<ChatSessionDto>> GetSessionsByModelIdAsync(Guid modelId)
        {
            var sessions = await _unitOfWork.ChatSessions.GetSessionsByModelIdAsync(modelId);
            return _mapper.Map<IEnumerable<ChatSessionDto>>(sessions);
        }

        public async Task<IEnumerable<ChatSessionDto>> GetActiveSessionsAsync()
        {
            var sessions = await _unitOfWork.ChatSessions.GetActiveSessions();
            return _mapper.Map<IEnumerable<ChatSessionDto>>(sessions);
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