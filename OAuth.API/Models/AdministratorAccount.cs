namespace OAuthAPI.Models
{
    public partial class AdministratorAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string AdminSecret { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
