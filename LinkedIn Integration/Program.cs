using LinkedIn_Integration;
using LinkedIn_Integration.Entities;
using LinkedIn_Integration.Services;
using LinkedIn_Integration.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Configure Swagger to use OAuth2 authentication
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://www.linkedin.com/oauth/v2/authorization", UriKind.Absolute),
                TokenUrl = new Uri("https://www.linkedin.com/oauth/v2/accessToken", UriKind.Absolute),
                Scopes = new Dictionary<string, string>
                    {
                        { "w_member_socal", "Read access" }
                    }
            }
        },
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                     {
                        new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                        Id = "Bearer", //The name of the previously defined security scheme.
                        Type = ReferenceType.SecurityScheme
                     }
                          },new List<string>()
                       }
    });

    // Configure Swagger to use OAuth2 for authorization
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
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
//builder.Services.AddAuthentication(opt =>
//{
//    opt.DefaultScheme = "OAuthProvider";
//    opt.DefaultChallengeScheme = "OAuthProvider";
//    opt.DefaultAuthenticateScheme = "OAuthProvider";
//}).AddOAuth("OAuthProvider", options =>
//{
//    options.ClientId = "77tqluk58xcp41";
//    options.ClientSecret = "fis8HjLFXgr3XYOn";
//    options.CallbackPath = "/https://oauth.pstmn.io/v1/callback";
//    options.AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization";
//    options.TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
//    options.SaveTokens = true;

//});

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOAuth("OAuthProvider", options =>
    {
        options.ClientId = "77tqluk58xcp41";
        options.ClientSecret = "fis8HjLFXgr3XYOn";
        options.CallbackPath = "/swagger/oauth2-redirect.html";
        options.Scope.Add("r_liteprofile");
        options.Scope.Add("r_emailaddress");
        options.AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization";
        options.TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                // Retrieve user information from LinkedIn
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.linkedin.com/v2/me");
                request.Headers.Add("x-li-format", "json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var email = json.RootElement.GetProperty("emailAddress").GetString();
                var name = json.RootElement.GetProperty("localizedFirstName").GetString();

                // Add user information to the authentication ticket
                var identity = (ClaimsIdentity)context.Principal.Identity;
                identity.AddClaim(new Claim(ClaimTypes.Email, email));
                identity.AddClaim(new Claim(ClaimTypes.Name, name));
            }
        };
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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
        c.OAuthClientId("77tqluk58xcp41");
        c.OAuthClientSecret("fis8HjLFXgr3XYOn");
        c.OAuthAppName("Your API - Swagger");
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        c.OAuth2RedirectUrl("https://oauth.pstmn.io/v1/browser-callback");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
