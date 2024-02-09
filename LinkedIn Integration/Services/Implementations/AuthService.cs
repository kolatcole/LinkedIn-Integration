using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpEntities.HttpResponses;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace LinkedIn_Integration.Services.Implementations
{
    public class AuthService(IOptions<LinkedInOptions> _options, IOptions<AuthOptions> _authOptions) : IAuthService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly LinkedInOptions options = _options.Value;
        private readonly AuthOptions authOptions = _authOptions.Value;

        public async Task<string> AuthRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{options.BaseURL}oauth/v2/authorization?response_type=code&" +
                                                $"client_id={authOptions.ClientId}&redirect_uri={authOptions.RedirectURI}&scope={authOptions.Scope}");

            return Helper.ExecuteAsync(request, client).Result.RequestMessage.RequestUri.ToString();
        }

        public async Task<string> GetTokens(string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseURL}oauth/v2/accessToken");
            var collection = new Dictionary<string, string>();
            collection.Add("grant_type", "authorization_code");
            collection.Add("code", code);
            collection.Add("client_id", authOptions.ClientId);
            collection.Add("client_secret", authOptions.Secret);
            collection.Add("redirect_uri", authOptions.RedirectURI);
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            return await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTokenWithRefreshToken(string refreshToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseURL}oauth/v2/accessToken");
            var collection = new Dictionary<string, string>();
            collection.Add("grant_type", "refresh_token");
            collection.Add("refresh_token", refreshToken);
            collection.Add("client_id", authOptions.ClientId);
            collection.Add("client_secret", authOptions.Secret);
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            return await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();
        }
        
    }
}
