using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GoldPriceMicroservice.Interfaces;
using Newtonsoft.Json.Linq;

namespace GoldPriceMicroservice
{
    public class ExternalPriceCalloutService : IExternalPriceCalloutService
    {
        private static HttpClient _client; 
        private readonly string _url = "https://api.metals.live/v1/spot/gold";
        
        public ExternalPriceCalloutService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient(); 
        }

        public async Task<string> GetLatestGoldSpotPriceAsync()
        {
            var response = await _client.GetAsync(_url);
            response.EnsureSuccessStatusCode();
            return await GetPriceFromResponse(response); 
        }

        private async Task<string> GetPriceFromResponse(HttpResponseMessage response)
        {
           var jsonString = await response.Content.ReadAsStringAsync();
           JArray jArray = JArray.Parse(jsonString);
           var price = jArray.FirstOrDefault()?["price"]?.Value<string>();
           return price; 
        }
    }
}
