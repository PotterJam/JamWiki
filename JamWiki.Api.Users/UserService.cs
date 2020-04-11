using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Google.Apis.Auth;
using JamWiki.Api.Config;

namespace JamWiki.Api.Users
{
    public class UserService : IUserService
    {
        private readonly SecurityConfiguration _securityConfiguration;
        private readonly IUserStore _userStore;

        public UserService(IUserStore userStore, SecurityConfiguration securityConfiguration)
        {
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _securityConfiguration = securityConfiguration ?? throw new ArgumentNullException(nameof(securityConfiguration));;
        }

        public async Task<WikiUser> GetOrCreateUser(GoogleJsonWebSignature.Payload payload)
        {
            var user = await _userStore.GetUser(payload);
            if (user == null)
            {
                return await _userStore.CreateUser(payload);
            }

            return user;
        }

        public Task<WikiUser> GetWikiUser(IPrincipal principal)
        {
            var identity = (ClaimsIdentity) principal.Identity;
            var claims = identity.Claims.ToList();

            var encryptedId = GetClaim(claims, JwtRegisteredClaimNames.Sub);
            var encryptedName = GetClaim(claims, JwtRegisteredClaimNames.GivenName);
            var encryptedEmail = GetClaim(claims, JwtRegisteredClaimNames.Email);

            return Task.FromResult(
                new WikiUser
                {
                    Id = Guid.Parse(DecryptJwtClaim(encryptedId)),
                    Name = DecryptJwtClaim(encryptedName),
                    Email = DecryptJwtClaim(encryptedEmail)
                });
            }
        
        private string GetClaim(IEnumerable<Claim> claims, string type)
        {
            var matchingClaim = claims.FirstOrDefault(x => string.Equals(x.Properties.Values.First(), type, StringComparison.OrdinalIgnoreCase));
            return matchingClaim?.Value;
        }

        private string DecryptJwtClaim(string encryptedClaim) =>
            Security.Security.Decrypt(_securityConfiguration.UserCredentialKey, encryptedClaim);
    }
}