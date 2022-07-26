namespace ClientApi.Models
{
    public partial class Client
    {
        public Client()
        {
            TokenValut = new HashSet<TokenValut>();
            EffectiveDate = DateTime.Now;
        }
        public string Id { get; set; }
        public string ClientName { get; set; }
        public string ClientSecret { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public virtual ICollection<TokenValut> TokenValut { get; set; }
    }
}