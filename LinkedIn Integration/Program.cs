using Azure;
using Azure.Core;
using LinkedIn_Integration;
using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using LinkedIn_Integration.Services.Implementations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Xml;
using static System.Net.WebRequestMethods;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<LinkedInOptions>(builder.Configuration.GetSection("LinkedInOptions"));
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("AuthOptions"));
builder.Services.AddSingleton<IPostService, PostService>();
builder.Services.AddSingleton<ICommentService, CommentService>();
builder.Services.AddSingleton<IEntityEngagementService, EntityEngagementService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<OAuthHandlerService>();
builder.Services.AddDbContext<ApplicationDbContext>(opt => {
    opt.UseSqlServer("Enter connectionstring here");
});
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{}).AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddUserManager<UserManager<AppUser>>();
var provider = builder.Services.BuildServiceProvider();
var dbContext = provider.GetRequiredService<ApplicationDbContext>();
var authOption = provider.GetRequiredService<AuthOptions>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.Cookie.Name = "AuthCookie"; 
    options.Cookie.HttpOnly = true; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
})
.AddOAuth("OAuthProvider", options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ClientId = authOption.ClientId;
    options.ClientSecret = authOption.Secret;
    options.CallbackPath = new PathString("/Account/linkedIn-response");
    options.Scope.Add("w_member_social");
    options.Scope.Add("r_basicprofile");
    options.Scope.Add("w_organization_social");
    options.Scope.Add("r_organization_social");
    options.Scope.Add("r_organization_social_feed");
    options.Scope.Add("w_organization_social_feed");
    options.Scope.Add("rw_organization_admin");
    
    
    
    options.SaveTokens = true;
    options.AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization";
    options.TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
    options.UserInformationEndpoint = "https://api.linkedin.com/v2/me";
    options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "localizedLastName");
    options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "localizedFirstName");
   options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "localizedFirstName");
    HttpClient client = new HttpClient();
    options.Events = new OAuthEvents
    {

        OnRedirectToAuthorizationEndpoint = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.RedirectUri.ToString());

            var resultUrl = Helper.ExecuteAsync(request, client).Result.RequestMessage.RequestUri.ToString();
            // Specify the path to the browser executable
            string browserPath = @"C:\Program Files\Internet Explorer\iexplore.exe";

            // Open the URL in the specified browser
            Process.Start(new ProcessStartInfo(browserPath, resultUrl));
        }
        ,
        OnCreatingTicket = async context =>
        {
            var oauthHandlerService = context.HttpContext.RequestServices.GetRequiredService<OAuthHandlerService>();
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            var linkedInUser = await response.Content.ReadFromJsonAsync<JsonElement>();

            var claims = new List<Claim>
            {
                new Claim("accessToken", context.AccessToken),
                new Claim("refreshToken", context.RefreshToken),
                new Claim("expiryTime", context.ExpiresIn.ToString()),
                new Claim("createPost","True")
                // new Claim("createdAt", context.)

            };
            // Create a new claim
            var createPostClaim = new Claim("createPost", "True");
            var resharePostClaim = new Claim("resharePost", "True");
            var deletePostClaim = new Claim("deletePost", "True");
            var getPostClaim = new Claim("getPost", "True");
            var updatePostClaim = new Claim("updatePost", "True");

            // Add the claim to the identity's claims collection
            ((ClaimsIdentity)context.Principal.Identity).AddClaim(createPostClaim);
            ((ClaimsIdentity)context.Principal.Identity).AddClaim(resharePostClaim);
            ((ClaimsIdentity)context.Principal.Identity).AddClaim(deletePostClaim);
            ((ClaimsIdentity)context.Principal.Identity).AddClaim(getPostClaim);
            ((ClaimsIdentity)context.Principal.Identity).AddClaim(updatePostClaim);


            var identity = new ClaimsIdentity(claims, "OAuthProvider");
            context.HttpContext.User.AddIdentity(identity);
            
            // Create a claims principal using the claims identity
            var principal = new ClaimsPrincipal(identity);
            var user = new AppUser
            {
                AccessToken = context.AccessToken,
                RefreshToken = context.RefreshToken,
                ExpiresIn = context.ExpiresIn.ToString(),
                UserName = linkedInUser.GetProperty("localizedFirstName").ToString()
            };

            var provider = builder.Services.BuildServiceProvider();
            var dbContext = provider.GetRequiredService<ApplicationDbContext>();
          
            var userStore = new UserStore<AppUser>(dbContext);
            IdentityOptions identityOptions = new IdentityOptions { };
            IOptions<IdentityOptions> identityOptionsAccessor = Options.Create(identityOptions);
            UserManager<AppUser> _userManager =new UserManager<AppUser>(userStore, null, null, null, null, null, null, null, null);
            IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory = new UserClaimsPrincipalFactory<AppUser>(_userManager, identityOptionsAccessor);
            await userClaimsPrincipalFactory.CreateAsync(user);
            HttpContextAccessor accessor = new HttpContextAccessor();
            var result = await _userManager.CreateAsync(user);

            SignInManager<AppUser> _signInManager = new SignInManager<AppUser>(_userManager,accessor, userClaimsPrincipalFactory, null, null, null, null);
            await _userManager.AddClaimAsync(user, createPostClaim);
            await _userManager.AddClaimAsync(user, resharePostClaim);
            await _userManager.AddClaimAsync(user, updatePostClaim);
            await _userManager.AddClaimAsync(user, deletePostClaim);
            await _userManager.AddClaimAsync(user, getPostClaim);

            var properties = new AuthenticationProperties
            {
                RedirectUri = "/Account/home"
            };
            IEnumerable<Claim> additionalClaims = new List<Claim>();
            if (result.Succeeded)
            {
                await _signInManager.SignInWithClaimsAsync(user, properties, additionalClaims);
            }
            await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            oauthHandlerService.CreatingTicketContext = context;
            Console.WriteLine(context.AccessToken + " token");
            Console.WriteLine(context.RefreshToken + " refresh token");
            Console.WriteLine("Ticket created");
            context.Response.Redirect("/Account/return-tokens");
        }

    };
});

builder.Services.AddAuthorization();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{ 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
public class OAuthHandlerService
{
    public OAuthCreatingTicketContext CreatingTicketContext { get; set; }
}