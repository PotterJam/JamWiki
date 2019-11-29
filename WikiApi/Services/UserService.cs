using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using WikiApi.Helpers;
using WikiApi.Stores.User;

namespace WikiApi.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserStore _userStore;

        public UserService(IUserStore userStore, IConfiguration configuration)
        {
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));;
        }

        public async Task<WikiUser> Authenticate(GoogleJsonWebSignature.Payload payload)
        {
            return await _userStore.GetUser(payload);
        }

        public Task<WikiUser> GetWikiUser(IPrincipal principal)
        {
            var identity = (ClaimsIdentity) principal.Identity;
            var claims = identity.Claims.ToList();

            var encryptedEmail = GetClaim(claims, JwtRegisteredClaimNames.Sub);
            var encryptedName = GetClaim(claims, JwtRegisteredClaimNames.GivenName);

            return Task.FromResult(
                new WikiUser
                {
                    Name = DecryptJwtClaim(encryptedEmail),
                    Email = DecryptJwtClaim(encryptedName)
                });
            }
        
        private string GetClaim(IEnumerable<Claim> claims, string type)
        {
            var matchingClaim = claims.FirstOrDefault(x => string.Equals(x.Properties.Values.First(), type, StringComparison.OrdinalIgnoreCase));
            return matchingClaim?.Value;
        }

        private string DecryptJwtClaim(string encryptedClaim) =>
            Security.Decrypt(_configuration["Auth:UserCredentials"], encryptedClaim);
    }
}