using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, List<string> roles);
        DateTime GetExpirationDate();
    }
}