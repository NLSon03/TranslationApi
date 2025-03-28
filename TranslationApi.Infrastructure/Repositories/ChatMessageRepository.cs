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
    public class ChatMessageRepository : Repository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesBySessionIdAsync(Guid sessionId)
        {
            return await _dbSet
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesBySenderTypeAsync(Guid sessionId, SenderType senderType)
        {
            return await _dbSet
                .Where(m => m.SessionId == sessionId && m.SenderType == senderType)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesByMessageTypeAsync(Guid sessionId, MessageType messageType)
        {
            return await _dbSet
                .Where(m => m.SessionId == sessionId && m.MessageType == messageType)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageResponseTimeAsync(Guid sessionId)
        {
            return await _dbSet
                .Where(m => m.SessionId == sessionId && m.ResponseTime.HasValue)
                .AverageAsync(m => m.ResponseTime ?? 0);
        }

        public async Task<ChatMessage?> GetLastMessageInSessionAsync(Guid sessionId)
        {
            return await _dbSet
                .Where(m => m.SessionId == sessionId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }
    }
}