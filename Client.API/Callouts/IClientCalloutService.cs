using ClientApi.Models;
using ClientApi.Responses;

namespace ClientApi.Callouts
{
    public interface IClientCalloutService
    {
        Task<DataResponse> GetDataAsync(string token, HttpContent request);
        Task<AuthenticatedClient> GetTokenAsync(HttpContent body);
        Task<AuthenticatedClient> GetTokenByRefreshTokenAsync();
    }
}
