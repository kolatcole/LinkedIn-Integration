using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpResponses;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinkedIn_Integration.Services
{
    public class LinkedInService(IOptions<LinkedInOptions> _options) : ILinkedInService
    {
        private readonly LinkedInOptions options = _options.Value;
        private readonly HttpClient client = new HttpClient();
        private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        public Task<bool> CommentOnPost(string postId, Comment comment)
        {
            throw new NotImplementedException();
        }

        private async Task<ImageUploadResponse> GetImageUploadResponse()
        {
            var body = new ImageUploadRequest
            {
                InitializeUploadRequest= new InitializeUploadRequest
                {
                    Owner = await GetOwner()
                }
            };

            var imageUploadResponse = await ExecuteGetAsync<ImageUploadResponse>("rest/images?action=initializeUpload", body, HttpMethod.Post);
            return imageUploadResponse;
        }
        private async Task<string> GetOwner()
        {
            var user = await ExecuteGetAsync<LinkedInUser>("v2/userinfo",null, HttpMethod.Get);
            return options.Owner + $"{user.Sub}";
        }
        private async Task<T> ExecuteGetAsync<T>(string Url, Object? body, HttpMethod? method)
        {
            var request = new HttpRequestMessage(method, options.BaseURL + Url);
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            if (body is not null)
            {
                var requestContent = JsonSerializer.Serialize((ImageUploadRequest)body, serializeOptions);
                request.Content = new StringContent(requestContent, null, "application/json");
            }

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                            $"Unable to get data at this moment");
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }
        public async Task<bool> CreatePost(Post post)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + $"rest/posts");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);

            // Text with images
            // Get the media filepaths from appsettings
            string[] filePaths;
            if (!string.IsNullOrEmpty(options.ImagePaths))
            {
                filePaths = GetImageFilePaths(options.ImagePaths);
                var images = new List<Image>();
                foreach (string path in filePaths)
                {
                    // call {{baseUrl}}/rest/images?action=initializeUpload first to get uploadUrl

                    var imageRespone = await GetImageUploadResponse();
                    // send a put request to uploadUrl with the filepath of the image

                    using (var imageContent = new MultipartFormDataContent())
                    {
                        var imageUploadRequest = new HttpRequestMessage(HttpMethod.Put, imageRespone.value.UploadUrl);
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
                        }
                        var image = new Image {
                            Id=imageRespone.value.Image
                        };

                        images.Add(image);
                    }

                }

                if (post.Content is null)
                    post.Content = new Content();

                if (post.Content.MultiImage is null)
                    post.Content.MultiImage = new MultiImage();

                post.Content.MultiImage.Images = images;
            }
                

            

            
            var content = JsonSerializer.Serialize(post, serializeOptions);
            request.Content = new StringContent(content, null, "application/json");
            var response = await client.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Console.WriteLine("Ok");
            }

            return true;
           

            //{
            //    "author": "urn:li:organization:5515715",
            //  "commentary": "Sample text Post",
            //  "visibility": "PUBLIC",
            //  "distribution": {
            //        "feedDistribution": "MAIN_FEED",
            //    "targetEntities": [],
            //    "thirdPartyDistributionChannels": []
            //  },
            //  "lifecycleState": "PUBLISHED",
            //  "isReshareDisabledByAuthor": false
            //}


            //{
            //  "author": "urn:li:organization:5515715",
            //  "commentary": "Sample video Post",
            //  "visibility": "PUBLIC",
            //  "distribution": {
            //        "feedDistribution": "MAIN_FEED",
            //    "targetEntities": [],
            //    "thirdPartyDistributionChannels": []
            //  },
            //  "media":{
            //        "title":"title of the video",
            //      "id": "urn:li:video:C5F10AQGKQg_6y2a4sQ"
            //      }
            //},
            //  "lifecycleState": "PUBLISHED",
            //  "isReshareDisabledByAuthor": false
            //}


            //{
            //    "author": "urn:li:organization:2414183",
            //    "commentary": "test multiimage post",
            //    "visibility": "PUBLIC",
            //    "distribution": {
            //        "feedDistribution": "MAIN_FEED",
            //        "targetEntities": [],
            //        "thirdPartyDistributionChannels": []
            //    },
            //    "lifecycleState": "PUBLISHED",
            //    "isReshareDisabledByAuthor": false,
            //    
            //}


            // Create MultiImage content

            //{
            //    "author": "urn:li:organization:2414183",
            //    "commentary": "test multiimage post",
            //    "visibility": "PUBLIC",
            //    "distribution": {
            //        "feedDistribution": "MAIN_FEED",
            //        "targetEntities": [],
            //        "thirdPartyDistributionChannels": []
            //    },
            //    "lifecycleState": "PUBLISHED",
            //    "isReshareDisabledByAuthor": false,
            //    "content": {
            //        "multiImage": {
            //            "images": [
            //                {
            //                "id": "urn:li:image:C4D22AQFttWMAaIqHaa",
            //                    "altText": "testing for alt tags1"
            //                },
            //                {
            //                "id": "urn:li:image:C4D22AQG7uz0yPJh588",
            //                    "altText": "testing for alt tags2"
            //                }
            //            ]
            //        }
            //    }
            //}
        }

        public Task<bool> DeletePost(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetPost(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePost(Post post)
        {
            throw new NotImplementedException();
        }
        private string[] GetImageFilePaths(string imagePaths)
        {
            return imagePaths.Split(",");
        }
    }
}
