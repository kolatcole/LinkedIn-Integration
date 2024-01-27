using System.Text.Json.Serialization;

namespace LinkedIn_Integration.Entities
{
    public class LinkedInUser
    {
        [JsonPropertyName("sub")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("given_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("family_name")]
        public string LastName { get; set; }
    }
}

