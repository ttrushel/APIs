using System.ComponentModel.DataAnnotations;

namespace OAuthAPI.Requests
{
    public class TokenRequest
    {
        [Required]
        public string ClientName { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
    }
}
