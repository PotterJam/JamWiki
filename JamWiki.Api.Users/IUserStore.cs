using System.Threading.Tasks;
using Google.Apis.Auth;

namespace JamWiki.Api.Stores.User
{
    public interface IUserStore
    {
        Task<WikiUser> GetUser(GoogleJsonWebSignature.Payload payload);
    }
}