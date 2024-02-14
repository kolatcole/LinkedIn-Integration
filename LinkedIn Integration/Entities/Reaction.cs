using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;

namespace LinkedIn_Integration.Entities
{
    public class EntityEngagement
    {
        [JsonPropertyName("totalShareStatistics")]
        public TotalShareStatistics TotalShareStatistics { get; set; }
        [JsonPropertyName("share")]
        public string Share { get; set; }
        [JsonPropertyName("organizationalEntity")]
        public string OrganizationalEntity { get; set; }
        

    }

    public class TotalShareStatistics
    {
        [JsonPropertyName("uniqueImpressionsCount")]
        public int UniqueImpressionsCount { get; set; }
        [JsonPropertyName("shareCount")]
        public int ShareCount { get; set; }
        [JsonPropertyName("engagement")]
        public decimal Engagement { get; set; }
        [JsonPropertyName("clickCount")]
        public int ClickCount { get; set; }
        [JsonPropertyName("likeCount")]
        public int LikeCount { get; set; }
        [JsonPropertyName("impressionCount")]
        public int ImpressionCount { get; set; }
        [JsonPropertyName("commentCount")]
        public int CommentCount { get; set; }
    }
}
