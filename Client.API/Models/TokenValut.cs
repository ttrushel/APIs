namespace ClientApi.Models
{
    public partial class TokenValut
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public virtual Client Client { get; set; }
    }
}
