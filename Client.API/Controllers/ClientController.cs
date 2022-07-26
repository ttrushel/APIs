using ClientApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers
{
    [ApiController]
    [Route("api/client")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;
        public ClientController(IClientService service)
        {
            _service = service;
        }

        [HttpGet("data")]
        public async Task<ActionResult> GetDataAsync()
        {

            var refreshToken = Request.Cookies["refreshToken"];

            var response = await _service.RunAsync(refreshToken);

            if (response != null && !string.IsNullOrEmpty(response.Client?.RefreshToken))
            {
                SetRefreshTokenInCookie(response.Client.RefreshToken);
            }

            return string.IsNullOrEmpty(response.ErrorMessage)
                ? StatusCode(StatusCodes.Status200OK, response.ResponseContent)
                : StatusCode(StatusCodes.Status500InternalServerError, response.ErrorMessage);
        }

        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}