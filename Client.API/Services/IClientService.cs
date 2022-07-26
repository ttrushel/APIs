using ClientApi.Responses;

namespace ClientApi.Services
{
    public interface IClientService
    {
        Task<DataResponse?> RunAsync(string refreshToken);
    }
}
