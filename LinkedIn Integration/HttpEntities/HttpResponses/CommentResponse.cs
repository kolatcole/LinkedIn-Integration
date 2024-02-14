using System.Text.Json.Serialization;

namespace LinkedIn_Integration.HttpEntities.HttpResponses
{
    public class CommentResponse
    {
        [JsonPropertyName("actor")]
        public string Actor { get; set; }
        [JsonPropertyName("agent")]
        public string Agent { get; set; }
        [JsonPropertyName("commentUrn")]
        public string CommentUrn { get; set; }

        [JsonPropertyName("created")]
        public CommentResponseCreated Created { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("lastModified")]
        public CommentResponseLastModified LastModified { get; set; }
        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("content")]
        public Content Content { get; set; }
        [JsonPropertyName("object")]
        public string Object { get; set; }
    }
    public class CommentResponseCreated
    {
        [JsonPropertyName("actor")]
        public string Actor { get; set; }
        [JsonPropertyName("time")]
        public string Time { get; set; }
    }
    public class CommentResponseLastModified
    {

        [JsonPropertyName("actor")]
        public string Actor { get; set; }
        [JsonPropertyName("time")]
        public string Time { get; set; }
    }
    public class Message
    {
        //[JsonPropertyName("attributes")]
        //public string Text { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
    public class Content
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("entity")]
        public Entity Entity { get; set; }
    }

    public class Entity
    {
        [JsonPropertyName("image")]
        public string Image { get; set; }
    }
}
