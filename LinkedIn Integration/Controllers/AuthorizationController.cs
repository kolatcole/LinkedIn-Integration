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
        [HttpGet("{token}/{organizationURN}/ {entityURN}")]
        public async Task<IActionResult> Get(string entityURN, string organizationURN, string token)
        {
        //    var response = await service.GetEngagements(entityURN, organizationURN, token);

            return Ok(response);
        }

    }
}
