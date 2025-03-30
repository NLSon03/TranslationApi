using AutoMapper;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;
using TranslationApi.Domain.Interfaces;

namespace TranslationApi.Application.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatMessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ChatMessageDto> AddMessageAsync(Guid sessionId, string content, SenderType senderType, MessageType messageType, long? responseTime = null)
        {
            var session = await _unitOfWork.ChatSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                throw new ArgumentException("Session not found", nameof(sessionId));
            }

            var message = new ChatMessage
            {
                SessionId = sessionId,
                Content = content,
                SenderType = senderType,
                MessageType = messageType,
                ResponseTime = responseTime
            };

            await _unitOfWork.ChatMessages.AddAsync(message);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ChatMessageDto>(message);
        }

        public async Task<ChatMessageDto?> GetMessageByIdAsync(Guid id)
        {
            var message = await _unitOfWork.ChatMessages.GetByIdAsync(id);
            return message != null ? _mapper.Map<ChatMessageDto>(message) : null;
        }

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesBySessionIdAsync(Guid sessionId)
        {
            var messages = await _unitOfWork.ChatMessages.GetMessagesBySessionIdAsync(sessionId);
            return _mapper.Map<IEnumerable<ChatMessageDto>>(messages);
        }

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesBySenderTypeAsync(Guid sessionId, SenderType senderType)
        {
            var messages = await _unitOfWork.ChatMessages.GetMessagesBySenderTypeAsync(sessionId, senderType);
            return _mapper.Map<IEnumerable<ChatMessageDto>>(messages);
        }

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesByMessageTypeAsync(Guid sessionId, MessageType messageType)
        {
            var messages = await _unitOfWork.ChatMessages.GetMessagesByMessageTypeAsync(sessionId, messageType);
            return _mapper.Map<IEnumerable<ChatMessageDto>>(messages);
        }

        public async Task<double> GetAverageResponseTimeAsync(Guid sessionId)
        {
            return await _unitOfWork.ChatMessages.GetAverageResponseTimeAsync(sessionId);
        }

        public async Task<ChatMessageDto?> GetLastMessageInSessionAsync(Guid sessionId)
        {
            var message = await _unitOfWork.ChatMessages.GetLastMessageInSessionAsync(sessionId);
            return message != null ? _mapper.Map<ChatMessageDto>(message) : null;
        }

        public async Task DeleteMessageAsync(Guid id)
        {
            var message = await _unitOfWork.ChatMessages.GetByIdAsync(id);
            if (message != null)
            {
                await _unitOfWork.ChatMessages.RemoveAsync(message);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task UpdateMessageContentAsync(Guid id, string newContent)
        {
            var message = await _unitOfWork.ChatMessages.GetByIdAsync(id);
            if (message != null)
            {
                message.Content = newContent;
                await _unitOfWork.ChatMessages.UpdateAsync(message);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}