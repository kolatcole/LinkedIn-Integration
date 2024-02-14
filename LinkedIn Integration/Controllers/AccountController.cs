using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AccountController(OAuthHandlerService oauthHandlerService, SignInManager<AppUser> signInManager,
                        UserManager<AppUser> userManager, ApplicationDbContext dbContext) : ControllerBase
    {
        private readonly OAuthHandlerService _oauthHandlerService = oauthHandlerService;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ApplicationDbContext _dbContext = dbContext;

        [Route("Login")]
        [HttpGet]
        public IActionResult LinkedInLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("home")
            };
            return Challenge(properties, "OAuthProvider");
        }

        [Route("linkedIn-response")]
        [HttpGet]
        public async Task<IActionResult> LinkedInResponse()
        {
           return Ok();
        }
        [Route("home")]
        [HttpGet]
        public async Task<IActionResult> home()
        {
            var context = _oauthHandlerService.CreatingTicketContext;
            return Ok(Created("", new
            {
                accessToken = context.AccessToken,
                refreshToken = context.RefreshToken,
                expiresIn = context.ExpiresIn
            }));
        }

        [Route("Logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [Route("LoginWithToken/{token}")]
        [HttpGet]
        public async Task<IActionResult> LoginWithToken(string token)
        {
            var user = await _dbContext.Users.Where(x => x.AccessToken == token).SingleOrDefaultAsync();

            if (user is null)
                return NotFound();


            await _signInManager.SignInAsync(user, new AuthenticationProperties { }, null);
            if (!await Helper.SignInPrincipal(_signInManager.Context, token))
                return Unauthorized();

            return Ok("Logged In");
        }
    }
}
