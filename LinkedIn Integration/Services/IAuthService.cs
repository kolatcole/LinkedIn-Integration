using LinkedIn_Integration.Entities;

namespace LinkedIn_Integration.Services
{
    public interface IAuthService
    {
        Task<HttpResponseMessage> AuthRequest(Auth auth);
        Task<HttpResponseMessage> TokenRequest(Auth auth);

    }
}
