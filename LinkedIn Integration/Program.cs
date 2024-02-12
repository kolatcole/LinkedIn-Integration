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
builder.Services.AddSwaggerGen(
//{
    //c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
    //    {
    //        new OpenApiSecurityScheme {
    //            Reference = new OpenApiReference {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "oauth2"
    //            },
    //            Scheme = "oauth2",
    //            Name = "oauth2",
    //            In = ParameterLocation.Header
    //        },
    //        new List <string> ()
    //    }
    //});

    //// Configure Swagger to use OAuth2 authentication
    //c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    //{
    //    Type = SecuritySchemeType.OAuth2,
    //    Flows = new OpenApiOAuthFlows
    //    {
    //        AuthorizationCode = new OpenApiOAuthFlow
    //        {
    //            AuthorizationUrl = new Uri("https://www.linkedin.com/oauth/v2/authorization"),
    //            TokenUrl = new Uri("https://www.linkedin.com/oauth/v2/accessToken"),
    //            Scopes = new Dictionary<string, string>
    //                {
    //                    { "w_member_social", "Read access" }
    //                }
    //        }
    //    },
    //});

    
    // Configure Swagger to use OAuth2 for authorization
   // c.OperationFilter<SecurityRequirementsOperationFilter>();
//}
);
//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme.",
//        Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
//        Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
//    });
//    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {

//    });
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
//                     {
//                        new OpenApiSecurityScheme{
//                        Reference = new OpenApiReference{
//                        Id = "Bearer", //The name of the previously defined security scheme.
//                        Type = ReferenceType.SecurityScheme
//                     }
//                          },new List<string>()
//                       }
//    });
//}) ;

builder.Services.Configure<LinkedInOptions>(builder.Configuration.GetSection("LinkedInOptions"));
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("AuthOptions"));
builder.Services.AddSingleton<IPostService, PostService>();
builder.Services.AddSingleton<ICommentService, CommentService>();
builder.Services.AddSingleton<IEntityEngagementService, EntityEngagementService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<OAuthHandlerService>();
builder.Services.AddDbContext<ApplicationDbContext>(opt => {
    opt.UseSqlServer("Data Source=DESKTOP-5SRAAK8\\SQLEXPRESS;Initial Catalog=CatalogDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
});
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{


}).AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddUserManager<UserManager<AppUser>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(/*"internal_cookie",*/ options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
})
.AddOAuth("OAuthProvider", options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ClientId = "77tqluk58xcp41";
    options.ClientSecret = "fis8HjLFXgr3XYOn";
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
   // options.ClaimActions.MapJsonKey("createPost", "True");
   options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "localizedFirstName");
    HttpClient client = new HttpClient();
    options.Events = new OAuthEvents
    {

        OnRedirectToAuthorizationEndpoint = async context =>
        {
            //Console.WriteLine(context.Request.ToString());
            //Console.WriteLine(context.RedirectUri.ToString());
            //Console.WriteLine(context.Response.ToString());

            Console.WriteLine("Redirected");

            var request = new HttpRequestMessage(HttpMethod.Get, context.RedirectUri.ToString());

            var resultUrl = Helper.ExecuteAsync(request, client).Result.RequestMessage.RequestUri.ToString();
            // Specify the path to the browser executable
            string browserPath = @"C:\Program Files\Internet Explorer\iexplore.exe";

            // Open the URL in the specified browser
            Process.Start(new ProcessStartInfo(browserPath, resultUrl));
            
            //var loginRequest = new HttpRequestMessage(HttpMethod.Get, result);

            //var loginResult = Helper.ExecuteAsync(loginRequest, client);
            //Console.WriteLine(loginResult.Result.Content.ReadAsStringAsync());
            // Console.WriteLine(result.StatusCode);
        }
        ,
        OnCreatingTicket = async context =>
        {
            var oauthHandlerService = context.HttpContext.RequestServices.GetRequiredService<OAuthHandlerService>();
            //var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            //var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            //var linkedInUser = await response.Content.ReadFromJsonAsync<JsonElement>();


            //string value = "{\"localizedLastName\":\"Kolawole\",\"profilePicture\":{\"displayImage\":\"urn:li:digitalmediaAsset:C4E03AQG18zzQ5Z48nA\"},\"firstName\":{\"localized\":{\"en_US\":\"Toheeb\"},\"preferredLocale\":{\"country\":\"US\",\"language\":\"en\"}},\"vanityName\":\"toheeb-kolawole\",\"lastName\":{\"localized\":{\"en_US\":\"Kolawole\"},\"preferredLocale\":{\"country\":\"US\",\"language\":\"en\"}},\"localizedHeadline\":\"Software Engineer with Azure DevOps expertise and Microsoft certifications\",\"id\":\"Mw2fYL2UDo\",\"headline\":{\"localized\":{\"en_US\":\"Software Engineer with Azure DevOps expertise and Microsoft certifications\"},\"preferredLocale\":{\"country\":\"US\",\"language\":\"en\"}},\"localizedFirstName\":\"Toheeb\"}";

            //// Deserialize the value into a JObject
            //JObject jsonObject = JObject.Parse(value);

            //// Serialize the JObject back to JSON
            //var json = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);

            //var linkedInUser = 

            //context.RunClaimActions(linkedInUser);
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
                UserName="Ankur"
                //UserName = linkedInUser.GetProperty("localizedFirstName").ToString()
            };

           


            var provider = builder.Services.BuildServiceProvider();
            var dbContext = provider.GetRequiredService<ApplicationDbContext>();

            //var userId = await dbContext.Users.AddAsync(user);
            //var d = await dbContext.SaveChangesAsync();

          
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
            //Seed.SaveClaims(dbContext,d.ToString());

            //// Sign in the user
            await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            
            //var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            //{
            //    claim.Issuer,
            //    claim.OriginalIssuer,
            //    claim.Type,
            //    claim.Value
            //});

            //new RedirectToActionResult("return-tokens", "Account", null);

            oauthHandlerService.CreatingTicketContext = context;
            Console.WriteLine(context.AccessToken + " token");
            Console.WriteLine(context.RefreshToken + " refresh token");
            //Console.WriteLine(json + "json");
            //Console.WriteLine(claims.FirstOrDefault().OriginalIssuer + "Issuer");
            //Console.WriteLine(claims.FirstOrDefault().Value + "value");
            Console.WriteLine("Ticket created");
            context.Response.Redirect("/Account/return-tokens");
            // Console.WriteLine(response.StatusCode);
        },
        OnTicketReceived = async context =>
        {
            Console.WriteLine("Ticket received");
        },
        OnAccessDenied = async context =>
        {
            Console.WriteLine("Access denied");
        },
        OnRemoteFailure = async context =>
        {
            Console.WriteLine("Remote failure");
        }

    };
});
var provider = builder.Services.BuildServiceProvider();
var dbContext = provider.GetRequiredService<ApplicationDbContext>();
//builder.Services.AddAuthorization();
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("createPost", p => p.RequireClaim("createPost", "True"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger(opt => {
    //    opt.RouteTemplate = "swagger/{documentName}/swagger.json";
    //});
    //app.UseSwaggerUI(opt => {
    //    opt.SwaggerEndpoint("v1/swagger.json", "First Api");
    //});

    app.UseSwagger();
    app.UseSwaggerUI(//c =>
    //{
    //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    //    c.OAuthClientId("77tqluk58xcp41");
    //    c.OAuthClientSecret("fis8HjLFXgr3XYOn");
    //    c.OAuthAppName("Your API - Swagger");
    //    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
    //    c.OAuth2RedirectUrl("https://oauth.pstmn.io/v1/browser-callback");
//    }
);
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

public static class Seed
{

    public static void SaveClaims(ApplicationDbContext dbContext, string userId)
    {
        var resharePostClaim = new IdentityUserClaim<string>();
        resharePostClaim.ClaimType = "resharePost";
        resharePostClaim.ClaimValue = "True";
        resharePostClaim.UserId = userId;

        var createPostClaim = new IdentityUserClaim<string>();
        createPostClaim.ClaimType = "createPost";
        createPostClaim.ClaimValue = "True";
        createPostClaim.UserId = userId;

        var deletePostClaim = new IdentityUserClaim<string>();
        deletePostClaim.ClaimType = "deletePost";
        deletePostClaim.ClaimValue = "True";
        deletePostClaim.UserId = userId;

        var getPostClaim = new IdentityUserClaim<string>();
        getPostClaim.ClaimType = "getPost";
        getPostClaim.ClaimValue = "True";
        getPostClaim.UserId = userId;

        var updatePostClaim = new IdentityUserClaim<string>();
        updatePostClaim.ClaimType = "updatePost";
        updatePostClaim.ClaimValue = "True";
        updatePostClaim.UserId = userId;

        dbContext.UserClaims.AddRangeAsync(new IdentityUserClaim<string>[] {createPostClaim,resharePostClaim,
            deletePostClaim,getPostClaim,updatePostClaim});
        dbContext.SaveChanges();

    }

    
}