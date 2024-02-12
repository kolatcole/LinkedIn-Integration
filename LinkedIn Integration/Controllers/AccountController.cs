using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace LinkedIn_Integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AccountController(OAuthHandlerService oauthHandlerService, SignInManager<AppUser> signInManager) : ControllerBase
    {
        private readonly OAuthHandlerService _oauthHandlerService = oauthHandlerService;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

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
            var user = this.User.Identity as ClaimsIdentity;
            //var token = HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "");
            return Content("hello dear");
            //var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //var claims =  result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            //{
            //    claim.Issuer,
            //    claim.OriginalIssuer,
            //    claim.Type,
            //    claim.Value
            //});
            //return Ok(claims);
            //return Redirect("https://api.linkedin.com/oauth/v2/accessToken");
            //return Ok();
        }
        [Route("home")]
        [HttpGet]
        public async Task<IActionResult> home()
        {
            //var creatingTicketContext = _oauthHandlerService.CreatingTicketContext;
            //var result = await HttpContext.AuthenticateAsync("OAuthProvider"); // Authenticate using the OAuth scheme
            //if (!result.Succeeded)
            //{
            //    // Handle authentication failure
            //    return BadRequest("Authentication failed");
            //}

            var context = _oauthHandlerService.CreatingTicketContext;
            return Ok(Created("", new
            {
                accessToken = context.AccessToken,
                refreshToken = context.RefreshToken,
                expiresIn = context.ExpiresIn
            }));
        }

        [HttpGet("Try")]
        [Authorize]
        public IActionResult Try()
        {
            return Ok();
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
    }
}
