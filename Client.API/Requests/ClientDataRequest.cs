using ClientApi.Models;

namespace ClientApi.Requests
{
    public class ClientDataRequest
    {
        public IEnumerable<ClientRequestData> RequestData { get; set; }
    }
}
