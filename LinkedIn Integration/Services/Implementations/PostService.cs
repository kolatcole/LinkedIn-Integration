using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpEntities.HttpRequests;
using LinkedIn_Integration.HttpEntities.HttpResponses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Content = LinkedIn_Integration.Entities.Content;

namespace LinkedIn_Integration.Services.Implementations
{
    public class PostService(IOptions<LinkedInOptions> _options) : IPostService
    {
        private readonly LinkedInOptions options = _options.Value;
        private readonly HttpClient client = new HttpClient();
        private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
       
        private async Task<ImageUploadResponse> GetImageUploadResponse()
        {
            var body = new ImageUploadRequest
            {
                InitializeUploadRequest = new InitializeUploadRequest
                {
                    Owner = await GetOwner()
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + "rest/images?action=initializeUpload");
            request.Headers.Add("Authorization", options.Token);
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
            var request = new HttpRequestMessage(HttpMethod.Get, options.BaseURL + "v2/userinfo");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var response = await Helper.ExecuteAsync(request, client).Result.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<LinkedInUser>(response);

            return options.Owner + $"{user.Sub}";
        }
        public async Task<HttpResponseMessage> CreatePost(Post post)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + $"rest/posts");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);


            // Get the media filepaths from appsettings
            string[] filePaths;
            if (!string.IsNullOrEmpty(options.ImagePaths))
            {
                filePaths = GetImageFilePaths(options.ImagePaths);

                await AddContentToPostAsync(filePaths, post);

            }
            var content = JsonSerializer.Serialize(post, serializeOptions);
            request.Content = new StringContent(content, null, "application/json");
            return await client.SendAsync(request);
        }
        public async Task<HttpResponseMessage> ResharePost(Post post)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseURL}rest/posts");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var content = JsonSerializer.Serialize(post, serializeOptions);
            request.Content = new StringContent(content, null, "application/json");

            return await Helper.ExecuteAsync(request,client);
        }
        public async Task<HttpResponseMessage> DeletePost(string Urn)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{options.BaseURL}rest/posts/{HttpUtility.UrlEncode(Urn)}");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);
            request.Headers.Add("X-RestLi-Method", "DELETE");
            return await Helper.ExecuteAsync(request, client);
        }
        public async Task<Post> GetPost(string Urn)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{options.BaseURL}rest/posts/{HttpUtility.UrlEncode(Urn)}/?viewContext=AUTHOR");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            var content = await Helper.ExecuteAsync(request,client).Result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Post>(content);
        }

        public async Task<HttpResponseMessage> UpdatePost(Post post, string shareUrn, string ugcPostUrn)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseURL}rest/posts/{shareUrn}/{ugcPostUrn}");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);
            request.Headers.Add("X-RestLi-Method:", "PARTIAL_UPDATE");

            var content = JsonSerializer.Serialize(post, serializeOptions);
            request.Content = new StringContent(content, null, "application/json");

            return await Helper.ExecuteAsync(request, client);
        }
        private string[] GetImageFilePaths(string imagePaths)
        {
            return imagePaths.Split(",");
        }
        private async Task AddContentToPostAsync(string[] filePaths, Post post)
        {
            ImageUploadResponse imageResponse;
            if (filePaths.Length == 1)
            {

                if (IsVideo(filePaths[0]))
                {
                    VideoUploadResponse mediaResponse;
                    // call {{baseUrl}}/rest/images?action=initializeUpload first to get uploadUrl

                    mediaResponse = await GetVideoUploadResponse();

                    // send a put request to uploadUrl with the filepath

                    using (var videoContent = new MultipartFormDataContent())
                    {
                        string eTag;
                        var videoUploadRequest = new HttpRequestMessage(HttpMethod.Put, mediaResponse.Value.UploadInstructions[0].UploadUrl);
                        //videoUploadRequest.Headers.Add("Authorization", options.Token);

                        using (var fileStream = File.OpenRead(filePaths[0]))
                        {
                            videoContent.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePaths[0]));

                            videoUploadRequest.Content = videoContent;
                            var videoUploadresponse = await client.SendAsync(videoUploadRequest);
                            eTag = videoUploadresponse.Headers.GetValues("ETag").ToArray()[0];
                            // Check the response
                            if (!videoUploadresponse.IsSuccessStatusCode)
                            {
                                throw new HttpRequestException("Unable to upload Video");
                            }
                        }

                        var finalizeUploadRequest = new FinalizeUploadRequest
                        {
                            Video = mediaResponse.Value.Video,
                            UploadedPartIds = new string[] { eTag },
                            UploadToken = ""
                        };
                        var finalizeVideoUploadRequest = new FinalizeVideoUploadRequest
                        {
                            FinalizeUploadRequest = finalizeUploadRequest
                        };


                        var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + "rest/videos?action=finalizeUpload");

                        // Headers
                        request.Headers.Add("Authorization", options.Token);
                        request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
                        request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

                        // Request content
                        var requestContent = JsonSerializer.Serialize(finalizeVideoUploadRequest, serializeOptions);
                        request.Content = new StringContent(requestContent, null, "application/json");


                        await Helper.ExecuteAsync(request,client);
                    }
                    post.Content = new Content();
                    post.Content.Media = new Media();
                    post.Content.Media.Id = mediaResponse.Value.Video;
                }
                else
                {
                    // call {{baseUrl}}/rest/images?action=initializeUpload first to get uploadUrl

                    imageResponse = await GetImageUploadResponse();
                    // send a put request to uploadUrl with the filepath

                    using (var imageContent = new MultipartFormDataContent())
                    {
                        var imageUploadRequest = new HttpRequestMessage(HttpMethod.Put, imageResponse.value.UploadUrl);
                        imageUploadRequest.Headers.Add("Authorization", options.Token);

                        using (var fileStream = File.OpenRead(filePaths[0]))
                        {
                            imageContent.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePaths[0]));

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
                }

            }
            else
            {
                var images = new List<Image>();
                foreach (string path in filePaths)
                {
                    // call {{baseUrl}}/rest/images?action=initializeUpload first to get uploadUrl

                    imageResponse = await GetImageUploadResponse();
                    // send a put request to uploadUrl with the filepath of the image

                    using (var imageContent = new MultipartFormDataContent())
                    {
                        var imageUploadRequest = new HttpRequestMessage(HttpMethod.Put, imageResponse.value.UploadUrl);
                        imageUploadRequest.Headers.Add("Authorization", options.Token);

                        using (var fileStream = File.OpenRead(path))
                        {
                            imageContent.Add(new StreamContent(fileStream), "file", Path.GetFileName(path));

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

                if (post.Content is null)
                    post.Content = new Content();

                if (post.Content.MultiImage is null)
                    post.Content.MultiImage = new MultiImage();

                post.Content.MultiImage.Images = images;
            }
        }

        private async Task<VideoUploadResponse> GetVideoUploadResponse()
        {

            var body = new VideoUploadRequest
            {
                InitializeUploadRequest = new InitializeVideoUploadRequest
                {
                    Owner = await GetOwner(),
                    FileSizeBytes = 1055736
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + "rest/videos?action=initializeUpload");
            request.Headers.Add("Authorization", options.Token);
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
            return path.Split(".")[1] == "mp4";
        }
    }
}
