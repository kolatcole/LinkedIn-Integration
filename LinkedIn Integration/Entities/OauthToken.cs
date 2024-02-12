namespace LinkedIn_Integration.Entities
{
    public class OauthToken
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}
