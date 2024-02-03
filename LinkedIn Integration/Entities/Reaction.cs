using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;

namespace LinkedIn_Integration.Entities
{
    public class Reaction
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("lastModified")]
        public LastModified lastModified { get; set; }
        [JsonPropertyName("reactionType")]
        public string ReactionType { get; set; }
        [JsonPropertyName("created")]
        public ResponseCreated Created { get; set; }
        [JsonPropertyName("root")]
        public string Root { get; set; }

    }

    public class LastModified
    {
        [JsonPropertyName("actor")]
        public string Actor { get; set; }
        [JsonPropertyName("time")]
        public long Time { get; set; }
    }

    public class ResponseCreated
    {
        [JsonPropertyName("actor")]
        public string Actor { get; set; }
        [JsonPropertyName("time")]
        public long Time { get; set; }
    }
}
