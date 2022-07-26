using OAuthAPI.Models;

namespace OAuthAPI.Repositories
{
    public interface IOAuthRepository
    {
        Task<AdministratorAccount?> AuthenticateAdministratorAsync(Administrator admin);
        Task<Client?> LoadClientAsync(ClientInformation client);
        Task<Client?> LoadClientByIdAsync(string clientId);
        Task<string?> LoadRefreshTokenClientIdAsync(string refreshToken);
        Task<string> StoreTokenAsync(TokenValut tokenInformation);
        Task<bool> SaveClientAsync(Client client);
    }
}