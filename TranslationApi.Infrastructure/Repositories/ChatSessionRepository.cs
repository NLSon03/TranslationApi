using Microsoft.EntityFrameworkCore;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Interfaces;
using TranslationApi.Infrastructure.Data;

namespace TranslationApi.Infrastructure.Repositories
{
    public class ChatSessionRepository : Repository<ChatSession>, IChatSessionRepository
    {
        public ChatSessionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();
        }

        public async Task<ChatSession?> GetSessionWithMessagesAsync(Guid sessionId)
        {
            return await _dbSet
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByModelIdAsync(Guid modelId)
        {
            return await _dbSet
                .Where(s => s.AIModelId == modelId)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatSession>> GetActiveSessions()
        {
            return await _dbSet
                .Where(s => !s.EndedAt.HasValue)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();
        }

        public async Task EndSessionAsync(Guid sessionId)
        {
            var session = await _dbSet.FindAsync(sessionId);
            if (session != null)
            {
                session.EndedAt = DateTime.UtcNow;
                await UpdateAsync(session);
            }
        }
    }
}