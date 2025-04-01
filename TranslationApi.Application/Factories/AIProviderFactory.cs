
using Microsoft.Extensions.DependencyInjection;
using TranslationApi.Application.Providers;
using TranslationApi.Application.Providers.Implementations;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Factories
{
    public class AIProviderFactory : IAIProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AIProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITranslationProvider CreateTranslationProvider(AIModel model)
        {
            return model.Provider.ToUpper() switch
            {
                "GEMINI" => ActivateProvider<GeminiTranslationProvider>(model),
                // Add more providers here
                _ => throw new NotSupportedException($"Translation provider {model.Provider} is not supported")
            };
        }

        public IChatProvider CreateChatProvider(AIModel model)
        {
            // TODO: Implement chat providers
            throw new NotImplementedException("Chat providers are not implemented yet");
            
            // Example implementation:
            //return model.Provider.ToUpper() switch
            //{
            //    "OPENAI" => ActivateProvider<OpenAIChatProvider>(model),
            //    _ => throw new NotSupportedException($"Chat provider {model.Provider} is not supported")
            //};
        }

        public IModelProvider CreateProvider(AIModel model)
        {
            return model.ModelType.ToUpper() switch
            {
                "TRANSLATION" => CreateTranslationProvider(model),
                "CHAT" => CreateChatProvider(model),
                _ => throw new NotSupportedException($"Model type {model.ModelType} is not supported")
            };
        }

        private T ActivateProvider<T>(AIModel model) where T : IModelProvider
        {
            var provider = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
            provider.Initialize(model.Config).Wait();
            return provider;
        }
    }
}