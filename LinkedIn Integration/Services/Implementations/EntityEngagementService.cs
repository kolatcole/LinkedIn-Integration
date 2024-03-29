﻿using LinkedIn_Integration.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Web;

namespace LinkedIn_Integration.Services.Implementations
{
    public class EntityEngagementService(IOptions<LinkedInOptions> _options) : IEntityEngagementService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly LinkedInOptions options = _options.Value;
       
        public async Task<EntityEngagement> GetEngagements(string entityUrn, string organizationUrn, string token)
        {
            HttpRequestMessage request;
            if(entityUrn.Contains("share"))
                request = new HttpRequestMessage(HttpMethod.Get, $"{options.BaseURL}rest/organizationalEntityShareStatistics?q=organizationalEntity&organizationalEntity={HttpUtility.UrlEncode(organizationUrn)}&shares=List({HttpUtility.UrlEncode(entityUrn)})");
            else
                request = new HttpRequestMessage(HttpMethod.Get, $"{options.BaseURL}rest/organizationalEntityShareStatistics?q=organizationalEntity&organizationalEntity={HttpUtility.UrlEncode(organizationUrn)}&ugcPosts=List({HttpUtility.UrlEncode(entityUrn)})");


            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);

            var content = await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();
            //return JsonSerializer.Deserialize<EntityEngagement>(content);

            // Parse the JSON response using System.Text.Json.JsonSerializer
            var jsonResponse = JsonSerializer.Deserialize<JsonDocument>(content);

            EntityEngagement engagements = new EntityEngagement();
            // Extract the "elements" array
            if (jsonResponse.RootElement.TryGetProperty("elements", out var elementsArray))
            {
                if(elementsArray.GetArrayLength() > 0)
                    engagements = JsonSerializer.Deserialize<EntityEngagement>(elementsArray[0]);
            }
            return engagements;
        }
    }
}
