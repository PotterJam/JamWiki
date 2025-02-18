﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using JamWiki.Api.Config;
using JamWiki.Api.Security;
using JamWiki.Api.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JamWiki.Auth
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        private readonly SecurityConfiguration _securityConfiguration;

        public AuthController(SecurityConfiguration securityConfiguration, IUserService userService)
        {
            _securityConfiguration = securityConfiguration ?? throw new ArgumentNullException(nameof(securityConfiguration));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        
        public class UserView
        {
            public string tokenId { get; set; }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Google([FromBody]UserView userView)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(userView.tokenId, new GoogleJsonWebSignature.ValidationSettings());
                var user = await _userService.GetOrCreateUser(payload);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, Security.Encrypt(_securityConfiguration.UserCredentialKey,user.Id.ToString())),
                    new Claim(JwtRegisteredClaimNames.Email, Security.Encrypt(_securityConfiguration.UserCredentialKey,user.Email)),
                    new Claim(JwtRegisteredClaimNames.GivenName, Security.Encrypt(_securityConfiguration.UserCredentialKey,user.Name)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_securityConfiguration.JwtSigningKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(string.Empty,
                    string.Empty,
                    claims,
                    expires: DateTime.Now.AddSeconds(55*60*24*14),
                    signingCredentials: credentials);
                
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return BadRequest();
        }
    }
}