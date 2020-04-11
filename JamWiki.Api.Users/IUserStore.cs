using System.Threading.Tasks;
using Google.Apis.Auth;

namespace JamWiki.Api.Users
{
    public interface IUserStore
    {
        Task<WikiUser> GetUser(GoogleJsonWebSignature.Payload payload);
        Task<WikiUser> CreateUser(GoogleJsonWebSignature.Payload payload);
    }
}