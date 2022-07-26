using Microsoft.EntityFrameworkCore;
using OAuthAPI.Models;

namespace OAuthAPI.Data
{
    public partial class OAuthContext : DbContext
    {
        public OAuthContext()
        {

        }
        public OAuthContext(DbContextOptions<OAuthContext> options) : base(options)
        {
        }
        public virtual DbSet<AdministratorAccount> AdministratorAccounts { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<TokenValut> TokenValut { get; set; }
    }
}
