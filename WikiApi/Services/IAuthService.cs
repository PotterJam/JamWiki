using System.Threading.Tasks;
using Google.Apis.Auth;

namespace WikiApi.Services
{
    public interface IAuthService
    {
        Task<User> Authenticate(GoogleJsonWebSignature.Payload payload);
    }
}