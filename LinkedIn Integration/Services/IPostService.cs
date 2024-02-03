using LinkedIn_Integration.Entities;

namespace LinkedIn_Integration.Services
{
    public interface IPostService
    {
        Task<HttpResponseMessage> CreatePost(Post post);
        Task<HttpResponseMessage> ResharePost(Post post);
        Task<HttpResponseMessage> UpdatePost(Post post, string shareUrn, string ugcPostUrn);

        Task<HttpResponseMessage> DeletePost(string Urn);

        Task<Post> GetPost(string Urn);
    }
}
