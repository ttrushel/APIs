using OAuthAPI.Models;
using OAuthAPI.Requests;

namespace OAuthAPI.Services
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterClientAsync(RegistrationRequest request);
        Task<AuthenticatedClient> GetTokenAsync(TokenRequest request);
        Task<AuthenticatedClient> RefreshTokenAsync(string token);
    }
}
