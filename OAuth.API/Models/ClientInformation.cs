namespace OAuthAPI.Models
{
    public class ClientInformation
    {
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public IEnumerable<TokenValut> RefreshTokens { get; set; }
    }
}
