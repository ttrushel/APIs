using ClientApi.Data;
using ClientApi.Helpers;
using ClientApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private ClientContext _dbContext;
        private ILoggerManager _logger;

        public ClientRepository(ClientContext dbContext, ILoggerManager logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Client?> GetClientCredentialsAsync()
        {
            return await _dbContext.Clients
                .Where(c => c.ClientName == "ClientName")
                .OrderByDescending(cli => cli.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ClientRequestData>> GetClientRequestDataAsync()
        {
            return await _dbContext.ClientRequestData
                .Where(record => record.Id != 0)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> SaveResponseDataAsync(ClientResponseData responseData)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                _dbContext.Add(responseData);
                await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.Log($"{ex.Message}", LogType.Error);
                return false;
            }
        }
    }
}

