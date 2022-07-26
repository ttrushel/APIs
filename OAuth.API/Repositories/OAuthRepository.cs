using OAuthAPI.Data;
using OAuthAPI.Helpers;
using OAuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace OAuthAPI.Repositories
{
    public class OAuthRepository : IOAuthRepository
    {
        private readonly OAuthContext _context;
        private IPasswordHasher<Client> _clientHasher;
        private IPasswordHasher<AdministratorAccount> _adminHasher;
        private readonly ILoggerManager _logger;

        public OAuthRepository(OAuthContext context, ILoggerManager logger)
        {
            _clientHasher = new PasswordHasher<Client>();
            _adminHasher = new PasswordHasher<AdministratorAccount>();
            _context = context;
            _logger = logger;
        }

        #region Public Methods

        public async Task<bool> SaveClientAsync(Client client)
        {
            var hashedSecret = _clientHasher.HashPassword(client, client.ClientSecret);
            client.ClientSecret = hashedSecret;
            var existingClient = await IsExistingClientAsync(client);

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (existingClient != null)
                {
                    client.Id = existingClient.Id;
                    _context.Update(client);
                }
                else
                {
                    _context.Add(client);
                }
                await _context.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.Log($"{ex.Message}", LogType.Error);
                return false;
            }
        }

        public async Task<string> StoreTokenAsync(TokenValut tokenInformation)
        {
            var existingClient = await LoadClientByIdAsync(tokenInformation.Client.Id);

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (existingClient != null)
                {
                    _context.Update(tokenInformation);
                }
                else
                {
                    _context.Add(tokenInformation);
                }
                await _context.SaveChangesAsync();
                transaction.Commit();
                return $"Token information saved sucessfully for ClientId {tokenInformation.Client.Id}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.Log($"{ex.Message}", LogType.Error);
                return ex.Message;
            }
        }

        public async Task<string?> LoadRefreshTokenClientIdAsync(string refreshToken)
        {
            return await _context.TokenValut
                .Where(r => r.RefreshToken.Equals(refreshToken) && r.ExpirationDate >= DateTime.Now)
                .AsNoTracking()
                .Select(c => c.Client.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<AdministratorAccount?> AuthenticateAdministratorAsync(Administrator admin)
        {
            var administrator = await _context.AdministratorAccounts
                .Where(a => a.Username.Equals(admin.UserName))
                .OrderByDescending(records => records.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return administrator != null
                ? await AuthenticateAdministratorAsync(administrator, administrator.AdminSecret, admin.AdminSecret)
                : administrator;
        }

        public async Task<Client?> LoadClientAsync(ClientInformation client)
        {
            var registeredClient = await _context.Clients
                .Where(c => c.Id.Equals(client.ClientId) && c.ClientName.Equals(client.ClientName))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return registeredClient != null
                ? await AuthenticateClientAsync(registeredClient, registeredClient.ClientSecret, client.ClientSecret)
                : registeredClient;
        }

        public async Task<Client?> LoadClientByIdAsync(string clientId)
        {
            return await _context.Clients
                .Where(c => c.Id.Equals(clientId))
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<Client?> IsExistingClientAsync(Client client)
        {
            return await _context.Clients
                .Where(c => c.Id.Equals(client.Id))
                .OrderByDescending(x => x.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        private async Task<AdministratorAccount?> AuthenticateAdministratorAsync(AdministratorAccount user, string hashedPassword, string password)
        {
            var isAuthenticated = _adminHasher.VerifyHashedPassword(user, hashedPassword, password);

            switch (isAuthenticated)
            {
                case PasswordVerificationResult.Success:
                    return user;
                case PasswordVerificationResult.SuccessRehashNeeded:
                    user.AdminSecret = password;
                    await UpdateAdministratorAsync(user);
                    return user;
                default:
                    _logger.Log($"Failed authentication for Administrator: {user.Username}.", LogType.Warn);
                    return null;
            }
        }
        private async Task<Client?> AuthenticateClientAsync(Client user, string hashedPassword, string password)
        {
            var isAuthenticated = _clientHasher.VerifyHashedPassword(user, hashedPassword, password);

            switch (isAuthenticated)
            {
                case PasswordVerificationResult.Success:
                    return user;
                case PasswordVerificationResult.SuccessRehashNeeded:
                    user.ClientSecret = password;
                    await SaveClientAsync(user);
                    return user;
                default:
                    _logger.Log($"Failed authentication for ClientId: {user.Id}.", LogType.Warn);
                    return null;
            }
        }

        private async Task<bool> UpdateAdministratorAsync(AdministratorAccount administrator)
        {
            var hashedSecret = _adminHasher.HashPassword(administrator, administrator.AdminSecret);
            administrator.AdminSecret = hashedSecret;

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Update(administrator);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"{ex.Message}", LogType.Error);
                return false;
            }
        }

        #endregion Private Methods

    }
}