using LinkedIn_Integration.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LinkedIn_Integration.Services
{
    public class LinkedInService(IOptions<LinkedInOptions> _options) : ILinkedInService
    {
        private readonly LinkedInOptions options = _options.Value;
        private readonly HttpClient client = new HttpClient();
        public Task<bool> CommentOnPost(string postId, Comment comment)
        {
            throw new NotImplementedException();
        }

        public async Task CreatePost(Post post)
        {

            //_client.BaseAddress = new Uri(options.BaseURL);
            //string route
            var request = new HttpRequestMessage(HttpMethod.Post, options.BaseURL + $"rest/posts");
            request.Headers.Add("Authorization", options.Token);
            request.Headers.Add("LinkedIn-Version", options.LinkedInVersion);
            request.Headers.Add("X-Restli-Protocol-Version", options.ProtocolVersion);
            //request.Headers.Add("Content-Type", "application/json");
            var content = JsonConvert.SerializeObject(post);
            request.Content = new StringContent(content, null, "application/json");
            var response = await client.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Console.WriteLine("Ok");
            }


           

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
    }
}
