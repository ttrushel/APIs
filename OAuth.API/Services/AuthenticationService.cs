using AutoMapper;
using OAuthAPI.Helpers;
using OAuthAPI.Models;
using OAuthAPI.Repositories;
using OAuthAPI.Requests;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OAuthAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings _jwt;
        private IOAuthRepository _oAuthRepository;
        private readonly ILoggerManager _logger;
        private IMapper _mapper;
        public AuthenticationService(IOptions<JwtSettings> jwt, IOAuthRepository oAuthRepository, ILoggerManager loggerManager, IMapper mapper)
        {
            _jwt = jwt.Value;
            _oAuthRepository = oAuthRepository;
            _logger = loggerManager;
            _mapper = mapper;
        }

        #region Public Methods

        public async Task<bool> RegisterClientAsync(RegistrationRequest request)
        {
            var admin = _mapper.Map<Administrator>(request);
            var administrator = await _oAuthRepository.AuthenticateAdministratorAsync(admin);
            if (administrator == null) { return false; }
            var registeredClient = _mapper.Map<Client>(request);

            return await _oAuthRepository.SaveClientAsync(registeredClient);
        }

        public async Task<AuthenticatedClient> GetTokenAsync(TokenRequest request)
        {
            var client = _mapper.Map<ClientInformation>(request);
            var existingClient = await _oAuthRepository.LoadClientAsync(client);
            return existingClient != null
                ? await SetAuthenticatedClientAsync(existingClient)
                : FailedAuthentication($"Client: {request.ClientName} has not been registered.");
        }

        public async Task<AuthenticatedClient> RefreshTokenAsync(string token)
        {
            var clientId = await _oAuthRepository.LoadRefreshTokenClientIdAsync(token);
            if (clientId == null)
            {
                return FailedAuthentication("ClientId does not exist or is not registered.");
            }

            var existingClient = await _oAuthRepository.LoadClientByIdAsync(clientId);
            if (existingClient == null)
            {
                return FailedAuthentication($"Failed to find client id: {clientId}.");
            }

            return await SetAuthenticatedClientAsync(existingClient);
        }

        #endregion Public Methods

        #region Private Methods 

        private async Task<AuthenticatedClient> SetAuthenticatedClientAsync(Client client)
        {
            var authenticatedClient = new AuthenticatedClient();

            JwtSecurityToken jwtSecurityToken = CreateJwtToken(client);
            var bearerToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var refreshToken = CreateRefreshToken(client, bearerToken);
            refreshToken.Client = client;

            authenticatedClient.Message = await _oAuthRepository.StoreTokenAsync(refreshToken);
            authenticatedClient.IsAuthenticated = true;
            authenticatedClient.ClientId = client.Id;
            authenticatedClient.Token = bearerToken;
            authenticatedClient.RefreshToken = refreshToken.RefreshToken;
            authenticatedClient.RefreshTokenExpiration = refreshToken.ExpirationDate;

            return authenticatedClient;
        }

        private AuthenticatedClient FailedAuthentication(string errorMessage)
        {
            _logger.Log(errorMessage, LogType.Warn);
            var authenticatedClient = new AuthenticatedClient();
            authenticatedClient.IsAuthenticated = false;
            authenticatedClient.Message = errorMessage;
            return authenticatedClient;
        }

        private TokenValut CreateRefreshToken(Client client, string bearerToken)
        {
            var generator = RandomNumberGenerator.Create();
            var randomNumber = new byte[32];
            generator.GetBytes(randomNumber);
            return new TokenValut
            {
                RefreshToken = Convert.ToBase64String(randomNumber),
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                EffectiveDate = DateTime.UtcNow,
                Client = client,
                Token = bearerToken
            };
        }

        private JwtSecurityToken CreateJwtToken(Client user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15.00),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        #endregion Private Methods 

    }
}

