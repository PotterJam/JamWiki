using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using WikiApi.Helpers;
using WikiApi.Services;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WikiApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class GoogleController : Controller
    {
        private IAuthService _authService;

        public GoogleController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
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
                //SimpleLogger.Log("userView = " + userView.tokenId);
                var payload = await GoogleJsonWebSignature.ValidateAsync(userView.tokenId, new GoogleJsonWebSignature.ValidationSettings());
                var user = await _authService.Authenticate(payload);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, Security.Encrypt("jwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecret",user.Email)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("jwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecretjwtsecret"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(String.Empty,
                    String.Empty,
                    claims,
                    expires: DateTime.Now.AddSeconds(55*60),
                    signingCredentials: creds);
                
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