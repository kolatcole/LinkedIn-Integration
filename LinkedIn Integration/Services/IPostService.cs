using LinkedIn_Integration.Entities;

namespace LinkedIn_Integration.Services
{
    public interface IPostService
    {
        Task<HttpResponseMessage> CreatePost(Post post, string token);
        Task<HttpResponseMessage> ResharePost(Post post, string token);
        Task<HttpResponseMessage> UpdatePost(PostUpdate post, string urn, string token);

        Task<HttpResponseMessage> DeletePost(string Urn, string token);

        Task<Post> GetPost(string Urn, string token);
    }
}
