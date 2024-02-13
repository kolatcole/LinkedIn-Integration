using System.Text.Json.Serialization;
using System.Text.Json;
using static AspNet.Security.OAuth.LinkedIn.LinkedInAuthenticationConstants;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace LinkedIn_Integration.Services
{
    public static class Helper
    {
        public static async Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage request, HttpClient client)
        {
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                            $"Unable to get data at this moment");
            }
            return response;
        }
        public static async Task<bool> SignInPrincipal(HttpContext httpContext, string token)
        {

            //CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)

            //var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, tokens.Response.RootElement);
               // OAuthCreatingTicketContext context = new OAuthCreatingTicketContext();
            var claims = new List<Claim>
            {
                new Claim("accessToken", token),
                //new Claim("refreshToken", context.RefreshToken),
                //new Claim("expiryTime", context.ExpiresIn.ToString()),
                new Claim("createPost","True")
                // new Claim("createdAt", context.)

            };


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            httpContext.User.AddIdentity(identity);

            // Create a claims principal using the claims identity
            var principal = new ClaimsPrincipal(identity);
            httpContext.User = principal;
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
          //  httpContext.Session.SetInt32("test", 1);
           
            return httpContext.User.Identity.IsAuthenticated;

        }
    }
}
