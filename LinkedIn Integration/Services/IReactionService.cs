using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpEntities.HttpResponses;

namespace LinkedIn_Integration.Services
{
    public interface IReactionService
    {
        Task<IEnumerable<Reaction>> GetReactions(string entityUrn);
    }
}
