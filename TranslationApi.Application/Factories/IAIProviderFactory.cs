
using TranslationApi.Application.Providers;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Factories
{
    public interface IAIProviderFactory
    {
        ITranslationProvider CreateTranslationProvider(AIModel model);
        IChatProvider CreateChatProvider(AIModel model);
        IModelProvider CreateProvider(AIModel model);
    }
}