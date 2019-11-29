using System.Security.Principal;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace WikiApi.Services
{
    public interface IUserService
    {
        Task<WikiUser> Authenticate(GoogleJsonWebSignature.Payload payload);
        Task<WikiUser> GetWikiUser(IPrincipal principal);
    }
}