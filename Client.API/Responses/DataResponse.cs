using ClientApi.Models;

namespace ClientApi.Responses
{
    public class DataResponse
    {
        public string ResponseContent { get; set; }
        public string ErrorMessage { get; set; }
        public AuthenticatedClient? Client { get; set; }
    }
}
