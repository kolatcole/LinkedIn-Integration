using System.Text.Json.Serialization;

namespace LinkedIn_Integration.HttpEntities.HttpResponses
{
    public class Value
    {
        [JsonPropertyName("uploadUrlExpiresAt")]
        public long UploadUrlExpiresAt { get; set; }
        [JsonPropertyName("uploadUrl")]
        public string UploadUrl { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
    }

    public class ImageUploadResponse
    {
        public Value value { get; set; }
    }

}
