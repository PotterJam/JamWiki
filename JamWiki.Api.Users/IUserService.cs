﻿using System.Security.Principal;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace JamWiki.Api.Users
{
    public interface IUserService
    {
        Task<WikiUser> GetOrCreateUser(GoogleJsonWebSignature.Payload payload);
        Task<WikiUser> GetWikiUser(IPrincipal principal);
    }
}