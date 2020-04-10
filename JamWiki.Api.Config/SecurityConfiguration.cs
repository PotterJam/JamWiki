namespace JamWiki.Api
{
    public class SecurityConfiguration
    {
        public string JwtSigningKey { get; set; }
        public string UserCredentialKey { get; set; }
    }
}