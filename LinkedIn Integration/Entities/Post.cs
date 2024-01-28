using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace LinkedIn_Integration.Entities
{
    public class Post
    {
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("commentary")]
        public string Commentary { get; set; }
        [JsonPropertyName("visibility")]
        public string Visibility { get; set; }
        [JsonPropertyName("distribution")]
        public Distribution Distribution { get; set; }
        [JsonPropertyName("lifecycleState")]
        public string LifecycleState { get; set; }
        [JsonPropertyName("isReshareDisabledByAuthor")]
        public bool IsReshareDisabledByAuthor { get; set; }
        [JsonPropertyName("media")]
        public Media? Media { get; set; }
        [JsonPropertyName("reshareContext")]
        public ReshareContext? ReshareContext { get; set; }
        [JsonPropertyName("content")]
        public Content? Content { get; set; }

    }

    public class Distribution
    {
        [JsonPropertyName("feedDistribution")]
        public string FeedDistribution { get; set; }
        //public IList TargetEntities { get; set; }
        //public IList ThirdPartyDistributionChannels { get; set; }
    }

    public class Media
    {
        public string Title { get; set; }
        public string Id { get; set; }
    }

    public class ReshareContext
    {
        public string Parent { get; set; }
    }


    public class Content
    {
        public MultiImage MultiImage { get; set; }
    }
    public class MultiImage
    {
        public IList<Image> Images { get; set; }
    }

    public class Image
    {
        public string Id { get; set; }
        public string AltText { get; set; }
    }
    public class Comment
    {

    }

    public class InitializeUploadRequest 
    {
        public string Owner { get; set; }
    }

    public class ImageUploadRequest
    {
        public InitializeUploadRequest InitializeUploadRequest { get; set; }
    }

}
