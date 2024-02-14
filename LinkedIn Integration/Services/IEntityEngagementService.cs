using LinkedIn_Integration.Entities;
using LinkedIn_Integration.HttpEntities.HttpResponses;

namespace LinkedIn_Integration.Services
{
    public interface IEntityEngagementService
    {
        Task<EntityEngagement> GetEngagements(string entityUrn, string organizationUrn, string token);
    }
}
