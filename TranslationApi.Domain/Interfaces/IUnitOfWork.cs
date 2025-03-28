using System;
using System.Threading.Tasks;

namespace TranslationApi.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAIModelRepository AIModels { get; }
        IChatSessionRepository ChatSessions { get; }
        IChatMessageRepository ChatMessages { get; }
        IFeedbackRepository Feedbacks { get; }
        
        Task<int> CompleteAsync();
    }
} 