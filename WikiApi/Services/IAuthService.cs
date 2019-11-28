using System.Threading.Tasks;
using Google.Apis.Auth;

namespace WikiApi.Services
{
    public interface IAuthService
    {
        Task<WikiUser> Authenticate(GoogleJsonWebSignature.Payload payload);
    }
}