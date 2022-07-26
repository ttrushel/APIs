using Microsoft.AspNetCore.Mvc;
using OAuthAPI.Helpers;
using OAuthAPI.Requests;
using OAuthAPI.Services;

namespace OAuthAPI.Controllers
{
    [Route("api/oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ILoggerManager _logger;

        public OAuthController(IAuthenticationService authService, ILoggerManager logger)
        {
            _authService = authService;
            _logger = logger;
        }

        #region Public Methods 

        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync(TokenRequest request)
        {
            var result = await _authService.GetTokenAsync(request);
            if (result.RefreshToken != null)
            {
                SetRefreshTokenInCookie(result.RefreshToken);
            }
            return Ok(result);
        }

        [HttpGet("refresh_token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return BadRequest("Empty refresh token");
            }

            var response = await _authService.RefreshTokenAsync(refreshToken);
            if (response.RefreshToken == null)
            {
                var errorMessage = "Failed to generate new token.";
                _logger.Log($"{errorMessage}", LogType.Error);
                return Problem($"{errorMessage}");
            }

            SetRefreshTokenInCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("register_client")]
        public async Task<ActionResult> RegisterClientAsync(RegistrationRequest request)
        {
            var isRegistered = await _authService.RegisterClientAsync(request);
            return isRegistered
                ? StatusCode(StatusCodes.Status200OK, $"{request.ClientId} has been successfully registered.")
                : StatusCode(StatusCodes.Status500InternalServerError, $"{request.ClientId} failed registration.");
        }

        #endregion Public Methods 

        #region Private Methods 

        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        #endregion Private Methods 
    }
}