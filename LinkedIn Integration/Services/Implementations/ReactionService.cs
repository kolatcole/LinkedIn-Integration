using LinkedIn_Integration.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using LinkedIn_Integration.HttpEntities.HttpResponses;
using System.Xml.Linq;
using System;

namespace LinkedIn_Integration.Services.Implementations
{
    public class ReactionService(IOptions<LinkedInOptions> _options) : IReactionService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly LinkedInOptions options = _options.Value;
       
        public async Task<IEnumerable<Reaction>> GetReactions(string entityUrn)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{options.BaseURL}rest/reactions/(entity:{entityUrn})?q=entity&sort=(value:REVERSE_CHRONOLOGICAL");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var responseContent = await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();

            // Parse the JSON response using System.Text.Json.JsonSerializer
            var jsonResponse = JsonSerializer.Deserialize<JsonDocument>(responseContent);

            IEnumerable<Reaction> reactions = new List<Reaction>();
            // Extract the "elements" array
            if (jsonResponse.RootElement.TryGetProperty("elements", out var elementsArray))
            {
                reactions = JsonSerializer.Deserialize<List<Reaction>>(elementsArray);
            }
            return reactions;
        }
    }
}
