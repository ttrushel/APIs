using ClientApi.Helpers;
using ClientApi.Models;
using ClientApi.Responses;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ClientApi.Callouts
{
    public class ClientCalloutService : IClientCalloutService
    {
        #region Private Methods

        private HttpClient _client;
        private string _relativeUrl = "";
        private ILoggerManager _logger;

        #endregion Private Methods

        #region Public Constructor 

        public ClientCalloutService(IHttpClientFactory clientFactory, ILoggerManager logger)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7243/");
        }

        #endregion Public Constructor 

        #region Public Methods

        public async Task<DataResponse> GetDataAsync(string token, HttpContent requestBody)
        {
            SetBearerToken(token);
            _relativeUrl = "api/secured/load_test";
            var result = await _client.PostAsync(_relativeUrl, requestBody);
            result.EnsureSuccessStatusCode();
            var response = new DataResponse
            {
                ResponseContent = await result.Content.ReadAsStringAsync()
            };
            return response;
        }

        public async Task<AuthenticatedClient> GetTokenAsync(HttpContent body)
        {
            _relativeUrl = "api/oauth/token";
            var tokenResponse = await _client.PostAsync(_relativeUrl, body);
            var jsonString = await tokenResponse.Content.ReadAsStringAsync();
            return ConvertToTokenResponse(jsonString);
        }

        public async Task<AuthenticatedClient> GetTokenByRefreshTokenAsync()
        {
            _relativeUrl = "api/oauth/refresh_token";
            var tokenResponse = await _client.GetAsync(_relativeUrl);
            var jsonString = await tokenResponse.Content.ReadAsStringAsync();
            return ConvertToTokenResponse(jsonString);
        }

        #endregion Public Methods

        #region Private Methods

        private AuthenticatedClient ConvertToTokenResponse(string result)
        {
            var response = JsonConvert.DeserializeObject<AuthenticatedClient>(result);
            if (response == null)
            {
                var errorMessage = "Deserialization of the token response is null.";
                _logger.Log($"{errorMessage}", LogType.Error);
                return new AuthenticatedClient { Message = $"{errorMessage}" };
            }
            return response;
        }
        private void SetBearerToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{token}");
        }

        #endregion Private Methods
    }
}