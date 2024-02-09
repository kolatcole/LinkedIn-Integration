using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController(ILogger<AuthorizationController> logger, IAuthService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetRedirectURL()
        {
            var response = await service.AuthRequest();
            return Ok(response);
        }

        [HttpPost("CreateToken/{code}")]
        public async Task<IActionResult> CreateToken(string code)
        {
            var response = await service.GetTokens(code);

            return Ok(response);
        }

        [HttpPost("CreateTokenWithRefreshToken/{refreshToken}")]
        public async Task<IActionResult> CreateTokenWithRefreshToken(string refreshToken)
        {
            var response = await service.GetTokenWithRefreshToken(refreshToken);

            return Ok(response);
        }


    }
}
