using System.ComponentModel.DataAnnotations;

namespace OAuthAPI.Requests
{
    public class RegistrationRequest
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        [Required]
        public string AdminUsername { get; set; }
        [Required]
        public string AdminPassword { get; set; }
    }
}
