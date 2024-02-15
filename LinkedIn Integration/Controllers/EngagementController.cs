using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EngagementController(IEntityEngagementService service,
                                        UserManager<AppUser> userManager,
                                            SignInManager<AppUser> signInManager) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

        [HttpGet("{organizationURN}/{entityURN}")]
        public async Task<IActionResult> Get(string entityURN, string organizationURN)
        {
            var token = _userManager.Users.Where(x => x.UserName == _signInManager.Context.User.Identity.Name).SingleOrDefault().AccessToken;
            var response = await service.GetEngagements(entityURN, organizationURN, token);

            return Ok(response);
        }

    }
}
