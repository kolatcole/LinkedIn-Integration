using System.Reflection.PortableExecutable;
using System.Text.Json.Serialization;

namespace LinkedIn_Integration.HttpEntities.HttpRequests
{
    public class VideoUploadRequest
    {
        public InitializeVideoUploadRequest InitializeUploadRequest { get; set; }
    }
    public class InitializeVideoUploadRequest
    {
        public string Owner { get; set; }
        public Int64 FileSizeBytes { get; set; }
        public bool UploadCaptions { get; set; }
        public bool UploadThumbnail { get; set; }
    }
    public class VideoUploadResponse
    {
        [JsonPropertyName("value")]
        public Value Value { get; set; }
    }


    public class Value
    {
      
        [JsonPropertyName("uploadUrlsExpireAt")]
        public Int64 UploadUrlsExpireAt { get; set; }
        [JsonPropertyName("video")]
        public string Video { get; set; }
        [JsonPropertyName("uploadInstructions")]
        public IList<UploadInstructions> UploadInstructions { get; set; }
        [JsonPropertyName("uploadToken")]
        public string UploadToken { get; set; }

    }
    public class UploadInstructions
    {
        [JsonPropertyName("uploadUrl")]
        public string UploadUrl { get; set; }
        [JsonPropertyName("lastByte")]
        public Int64 LastByte { get; set; }
        [JsonPropertyName("firstByte")]
        public Int64 FirstByte { get; set; }
    }
  
    public class FinalizeVideoUploadRequest
    {
        public FinalizeUploadRequest FinalizeUploadRequest { get; set; }
    }
    public class FinalizeUploadRequest
    {
        public string Video { get; set; }
        public string UploadToken { get; set; }
        public string[] UploadedPartIds { get; set; }
    }
}