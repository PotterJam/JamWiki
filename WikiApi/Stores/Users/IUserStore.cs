using System.Threading.Tasks;
using Google.Apis.Auth;

namespace WikiApi.Stores.User
{
    public interface IUserStore
    {
        Task<WikiUser> GetUser(GoogleJsonWebSignature.Payload payload);
    }
}