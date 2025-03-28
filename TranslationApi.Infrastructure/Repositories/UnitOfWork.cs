using System;
using System.Threading.Tasks;
using TranslationApi.Domain.Interfaces;
using TranslationApi.Infrastructure.Data;

namespace TranslationApi.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IAIModelRepository _aiModelRepository;
        private IChatSessionRepository _chatSessionRepository;
        private IChatMessageRepository _chatMessageRepository;
        private IFeedbackRepository _feedbackRepository;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IAIModelRepository AIModels => _aiModelRepository ??= new AIModelRepository(_context);

        public IChatSessionRepository ChatSessions => _chatSessionRepository ??= new ChatSessionRepository(_context);

        public IChatMessageRepository ChatMessages => _chatMessageRepository ??= new ChatMessageRepository(_context);

        public IFeedbackRepository Feedbacks => _feedbackRepository ??= new FeedbackRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
} 