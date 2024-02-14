using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpEntities.HttpResponses;

namespace LinkedIn_Integration.Services
{
    public interface ICommentService
    {
        Task<HttpResponseMessage> CreateComment(Comment comment, string postUrn, string token);
        Task<IEnumerable<Comment>> GetComments(string Urn, string token);
    }
}
