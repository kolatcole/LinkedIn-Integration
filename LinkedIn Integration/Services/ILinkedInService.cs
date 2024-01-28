using LinkedIn_Integration.Entities;

namespace LinkedIn_Integration.Services
{
    public interface ILinkedInService
    {
        Task<bool> CreatePost(Post post);
        Task UpdatePost(Post post);

        Task<bool> DeletePost(string id);

        //Re-Post Any Selected Post

        Task<bool> CommentOnPost(string postId, Comment comment);
        Task<Post> GetPost(string id);
    }
}
