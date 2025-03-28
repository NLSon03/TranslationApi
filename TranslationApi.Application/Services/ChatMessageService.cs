using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;
using TranslationApi.Domain.Interfaces;

namespace TranslationApi.Application.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatMessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ChatMessage> AddMessageAsync(Guid sessionId, string content, SenderType senderType, MessageType messageType, long? responseTime = null)
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
            return message;
        }

        public async Task<ChatMessage?> GetMessageByIdAsync(Guid id)
        {
            return await _unitOfWork.ChatMessages.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesBySessionIdAsync(Guid sessionId)
        {
            return await _unitOfWork.ChatMessages.GetMessagesBySessionIdAsync(sessionId);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesBySenderTypeAsync(Guid sessionId, SenderType senderType)
        {
            return await _unitOfWork.ChatMessages.GetMessagesBySenderTypeAsync(sessionId, senderType);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesByMessageTypeAsync(Guid sessionId, MessageType messageType)
        {
            return await _unitOfWork.ChatMessages.GetMessagesByMessageTypeAsync(sessionId, messageType);
        }

        public async Task<double> GetAverageResponseTimeAsync(Guid sessionId)
        {
            return await _unitOfWork.ChatMessages.GetAverageResponseTimeAsync(sessionId);
        }

        public async Task<ChatMessage?> GetLastMessageInSessionAsync(Guid sessionId)
        {
            return await _unitOfWork.ChatMessages.GetLastMessageInSessionAsync(sessionId);
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