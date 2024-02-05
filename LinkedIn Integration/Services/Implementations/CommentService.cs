using LinkedIn_Integration.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using LinkedIn_Integration.HttpEntities.HttpResponses;
using System.Xml.Linq;
using System.Web;

namespace LinkedIn_Integration.Services.Implementations
{
    public class CommentService(IOptions<LinkedInOptions> _options) : ICommentService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly LinkedInOptions options = _options.Value;
        public async Task<HttpResponseMessage> CreateComment(Comment comment, string postUrn)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseURL}rest/socialActions/{HttpUtility.UrlEncode(postUrn)}/comments");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", "202306");
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var content = JsonSerializer.Serialize(comment, serializeOptions);
            request.Content = new StringContent(content, null, "application/json");

            return await Helper.ExecuteAsync(request, client);
        }

        public async Task<IEnumerable<Comment>> GetComments(string Urn)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{options.BaseURL}rest/socialActions/{Urn}/comments");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var responseContent = await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();
                
                // Parse the JSON response using System.Text.Json.JsonSerializer
            var jsonResponse = JsonSerializer.Deserialize<JsonDocument>(responseContent);

            IEnumerable<Comment> comments = new List<Comment>();
            // Extract the "elements" array
            if (jsonResponse.RootElement.TryGetProperty("elements", out var elementsArray))
            {
                comments = JsonSerializer.Deserialize<List<Comment>>(elementsArray);
            }
            return comments;
        }
    }
}
