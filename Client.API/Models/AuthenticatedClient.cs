namespace ClientApi.Models
{
    public class AuthenticatedClient
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string ClientId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}