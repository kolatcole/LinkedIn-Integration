using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpEntities.HttpResponses;

namespace LinkedIn_Integration.Services
{
    public interface ICommentService
    {
        Task<HttpResponseMessage> CreateComment(Comment comment, string postUrn);
        Task<IEnumerable<Comment>> GetComments(string Urn);
    }
}
