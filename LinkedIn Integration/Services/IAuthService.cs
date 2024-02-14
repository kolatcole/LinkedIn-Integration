using LinkedIn_Integration.Entities;

namespace LinkedIn_Integration.Services
{
    public interface IAuthService
    {
        Task<string> AuthRequest();
        Task<string> GetTokens(string code);
        Task<string> GetTokenWithRefreshToken(string refreshToken);

    }
}
