using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpEntities.HttpRequests;
using LinkedIn_Integration.HttpEntities.HttpResponses;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Content = LinkedIn_Integration.Entities.Content;
using System.Net.Http;

namespace LinkedIn_Integration.Services.Implementations
{
    public class PostService(IOptions<LinkedInOptions> _options) : IPostService
    {
        private readonly LinkedInOptions options = _options.Value;
        private readonly HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(15)
        };

        private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
       
        private async Task<ImageUploadResponse> GetImageUploadResponse(string token)
        {
            var body = new ImageUploadRequest
            {
                InitializeUploadRequest = new InitializeUploadRequest
                {
                    Owner = await GetOwner()
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + "rest/images?action=initializeUpload");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var requestContent = JsonSerializer.Serialize(body, serializeOptions);
            request.Content = new StringContent(requestContent, null, "application/json");

            //var imageUploadResponse = await Helper.ExecuteAsync(request,client);

            var content = await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ImageUploadResponse>(content);
        }
        private async Task<string> GetOwner()
        {
            //var request = new HttpRequestMessage(HttpMethod.Get, options.BaseURL + "v2/userinfo");
            //request.Headers.Add("Authorization", options.Token);
            //request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            //request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            //var response = await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();
            //var user = JsonSerializer.Deserialize<LinkedInUser>(response);

            //return options.Owner + $"{user.Sub}";
            return options.Owner;
        }
        public async Task<HttpResponseMessage> CreatePost(Post post, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + $"rest/posts");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);


            //// Get the media filepaths from appsettings
            //string[] filePaths;
            //if (!string.IsNullOrEmpty(options.ImagePaths))
            //{
            //    filePaths = GetImageFilePaths(options.ImagePaths);

            //    await AddContentToPostAsync(filePaths, post);

            //}

            // Get the media filepaths from json
            string[] filePaths;
            if (post.Content is not null && post.Content.Media is not null && !string.IsNullOrEmpty(post.Content.Media.Title))
            {
                filePaths = GetImageFilePaths(post.Content.Media.Title);

                await AddContentToPostAsync(filePaths, post, token);               

            }

            var content = JsonSerializer.Serialize(post, serializeOptions);
            request.Content = new StringContent(content, null, "application/json");

            return await Helper.ExecuteAsync(request, client);
        }
        public async Task<HttpResponseMessage> ResharePost(Post post, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseURL}rest/posts");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var content = JsonSerializer.Serialize(post, serializeOptions);
            request.Content = new StringContent(content, null, "application/json");

            return await Helper.ExecuteAsync(request,client);
        }
        public async Task<HttpResponseMessage> DeletePost(string Urn, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{options.BaseURL}rest/posts/{HttpUtility.UrlEncode(Urn)}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);
            request.Headers.Add("X-RestLi-Method", "DELETE");
            return await Helper.ExecuteAsync(request, client);
        }
        public async Task<Post> GetPost(string Urn, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{options.BaseURL}rest/posts/{HttpUtility.UrlEncode(Urn)}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var content = await Helper.ExecuteAsync(request,client).Result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Post>(content);
        }

        public async Task<HttpResponseMessage> UpdatePost(PostUpdate post, string urn, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseURL}rest/posts/{HttpUtility.UrlEncode(urn)}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);
            request.Headers.Add("X-RestLi-Method", "PARTIAL_UPDATE");

            var content = JsonSerializer.Serialize(post, serializeOptions);
            request.Content = new StringContent(content, null, "application/json");

            return await Helper.ExecuteAsync(request, client);
        }
        private string[] GetImageFilePaths(string imagePaths)
        {
            return imagePaths.Split(",");
        }
        private async Task AddContentToPostAsync(string[] filePaths, Post post, string token)
        {
            
            ImageUploadResponse imageResponse;
            if (filePaths.Length == 1)
            {
                if (IsVideo(post.Content.Media.Title))
                {
                    VideoUploadResponse mediaResponse;
                    // call {{baseUrl}}/rest/images?action=initializeUpload first to get uploadUrl
                    var downloadedFile = await DownloadMediaAsync(filePaths[0]);
                    mediaResponse = await GetVideoUploadResponse(token, downloadedFile.Content.Headers.ContentLength.Value);

                    //previous video
                    var fileStream = downloadedFile.Content.ReadAsStream();
                    long chunkSize = 4 * 1024 * 1024;
                    byte[] buffer = new byte[chunkSize];
                    int bytesRead;
                    int chunkNumber = 0;
                    List<string> eTags = new List<string>();

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        chunkNumber++;
                        ByteArrayContent byteContent = new ByteArrayContent(buffer, 0, bytesRead);

                        // Add headers for chunked upload
                        long startByte = (chunkNumber - 1) * chunkSize; 
                        long endByte = startByte + bytesRead - 1; 
                        byteContent.Headers.Add("Content-Range", $"bytes {startByte}-{endByte}/{fileStream.Length}");
                        byteContent.Headers.Add("Content-Type", "application/octet-stream");

                        // Send the chunk to the server
                        HttpResponseMessage videoUploadresponse = await client.PostAsync(mediaResponse.Value.UploadInstructions[chunkNumber - 1].UploadUrl, byteContent);

                        string eTag = videoUploadresponse.Headers.GetValues("ETag").ToArray()[0];
                        // Check the response
                        if (!videoUploadresponse.IsSuccessStatusCode)
                        {
                            throw new HttpRequestException("Unable to upload Video");
                        }
                        eTags.Add(eTag);
                    }
                  
                    var finalizeUploadRequest = new FinalizeUploadRequest
                    {
                        Video = mediaResponse.Value.Video,
                        UploadedPartIds = eTags.ToArray(),
                        UploadToken = ""
                    };
                    var finalizeVideoUploadRequest = new FinalizeVideoUploadRequest
                    {
                        FinalizeUploadRequest = finalizeUploadRequest
                    };


                    var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + "rest/videos?action=finalizeUpload");

                    // Headers
                    request.Headers.Add("Authorization", $"Bearer {token}");
                    request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
                    request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

                    // Request content
                    var requestContent = JsonSerializer.Serialize(finalizeVideoUploadRequest, serializeOptions);
                    request.Content = new StringContent(requestContent, null, "application/json");


                    await Helper.ExecuteAsync(request, client);
                    post.Content = new Content();
                    post.Content.Media = new Media();
                    post.Content.Media.Id = mediaResponse.Value.Video;
                    post.Content.MultiImage = null;
                   
                }
                else
                {
                    // call {{baseUrl}}/rest/images?action=initializeUpload first to get uploadUrl

                    imageResponse = await GetImageUploadResponse(token);
                    // send a put request to uploadUrl with the filepath

                    // upload from desktop
                    //using (var imageContent = new MultipartFormDataContent())
                    //{
                    //    var imageUploadRequest = new HttpRequestMessage(HttpMethod.Put, imageResponse.value.UploadUrl);
                    //    imageUploadRequest.Headers.Add("Authorization", options.Token);

                    //    using (var fileStream = File.OpenRead(filePaths[0]))
                    //    {
                    //        imageContent.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePaths[0]));

                    //        imageUploadRequest.Content = imageContent;
                    //        var imageUploadresponse = await client.SendAsync(imageUploadRequest);

                    //        // Check the response
                    //        if (imageUploadresponse.StatusCode != HttpStatusCode.Created)
                    //        {
                    //            throw new HttpRequestException("Unable to upload image");
                    //        }
                    //    }
                    //}

                    // upload from web
                    using (var imageContent = new MultipartFormDataContent())
                    {
                        var imageUploadRequest = new HttpRequestMessage(HttpMethod.Put, imageResponse.value.UploadUrl);
                        imageUploadRequest.Headers.Add("Authorization", $"Bearer {token}");

                        using (var downloadedFile =await DownloadMediaAsync(filePaths[0]))
                        {
                            
                            imageContent.Add(new StreamContent(downloadedFile.Content.ReadAsStream()), "file", "image1.jpg");

                            imageUploadRequest.Content = imageContent;
                            var imageUploadresponse = await client.SendAsync(imageUploadRequest);

                            // Check the response
                            if (imageUploadresponse.StatusCode != HttpStatusCode.Created)
                            {
                                throw new HttpRequestException("Unable to upload image");
                            }
                        }
                    }
                    post.Content = new Content();
                    post.Content.Media = new Media();
                    post.Content.Media.Id = imageResponse.value.Image;
                    post.Content.MultiImage = null;
                }

            }
            else
            {
                int i = 0;
                var images = new List<Image>();
                foreach (string path in filePaths)
                {
                    // call {{baseUrl}}/rest/images?action=initializeUpload first to get uploadUrl

                    imageResponse = await GetImageUploadResponse(token);
                    // send a put request to uploadUrl with the filepath of the image

                    using (var imageContent = new MultipartFormDataContent())
                    {
                        var imageUploadRequest = new HttpRequestMessage(HttpMethod.Put, imageResponse.value.UploadUrl);
                        imageUploadRequest.Headers.Add("Authorization", $"Bearer {token}");

                        using (var downloadedFile = await DownloadMediaAsync(path))
                        {
                            imageContent.Add(new StreamContent(downloadedFile.Content.ReadAsStream()), "file", $"image{i+=1}");

                            imageUploadRequest.Content = imageContent;
                            var imageUploadresponse = await client.SendAsync(imageUploadRequest);

                            // Check the response
                            if (imageUploadresponse.StatusCode != HttpStatusCode.Created)
                            {
                                throw new HttpRequestException("Unable to upload image");
                            }

                            var image = new Image
                            {
                                Id = imageResponse.value.Image
                            };

                            images.Add(image);
                        }
                    }
                }
                post.Content.Media = null;
                if (post.Content is null)
                    post.Content = new Content();

                if (post.Content.MultiImage is null)
                    post.Content.MultiImage = new MultiImage();

                post.Content.MultiImage.Images = images;
            }
        }

        //previous video
        private async Task<VideoUploadResponse> GetVideoUploadResponse(string token, long fileSize)
        {

            var body = new VideoUploadRequest
            {
                InitializeUploadRequest = new InitializeVideoUploadRequest
                {
                    Owner = await GetOwner(),
                    FileSizeBytes = fileSize
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + "rest/videos?action=initializeUpload");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var requestContent = JsonSerializer.Serialize(body, serializeOptions);
            request.Content = new StringContent(requestContent, null, "application/json");


            //var videoUploadResponse = await Helper.ExecuteAsync(request, client);
            var content = await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<VideoUploadResponse>(content);
        }
        private bool IsVideo(string path)
        {
            return !path.Contains(".jpg") && !path.Contains(".jpeg") && !path.Contains(".png");
        }
        private async Task<HttpResponseMessage> DownloadMediaAsync(string url)
        {
            //return await client.GetStreamAsync(url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return await Helper.ExecuteAsync(request, client);
        }
    }
}
