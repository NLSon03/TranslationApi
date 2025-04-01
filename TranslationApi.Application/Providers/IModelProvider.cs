
namespace TranslationApi.Application.Providers
{
    public interface IModelProvider
    {
        Task<bool> ValidateConfig(string config);
        Task Initialize(string config);
        string ProviderName { get; }
    }
}