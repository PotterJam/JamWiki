using System.Security.Principal;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace JamWiki.Api.Services
{
    public interface IUserService
    {
        Task<WikiUser> Authenticate(GoogleJsonWebSignature.Payload payload);
        Task<WikiUser> GetWikiUser(IPrincipal principal);
    }
}