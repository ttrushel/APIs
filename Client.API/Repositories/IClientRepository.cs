using ClientApi.Models;

namespace ClientApi.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetClientCredentialsAsync();
        Task<IEnumerable<ClientRequestData>> GetClientRequestDataAsync();
        Task<bool> SaveResponseDataAsync(ClientResponseData responseData);
    }
}
