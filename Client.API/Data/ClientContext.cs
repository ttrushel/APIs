using ClientApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Data
{
    public class ClientContext : DbContext
    {
        public ClientContext()
        {
        }
        public ClientContext(DbContextOptions<ClientContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<TokenValut> TokenValut { get; set; }
        public DbSet<ClientResponseData> ClientResponseData { get; set; }
        public DbSet<ClientRequestData> ClientRequestData { get; set; }

    }
}
