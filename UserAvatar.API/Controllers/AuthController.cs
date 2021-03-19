using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using UserAvatar.API.Contracts;
using UserAvatar.API.Options;
using UserAvatar.DAL.Entities;

namespace UserAvatar.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtOptions _jwt;

        public AuthController(IOptions<JwtOptions> jwt)
        {
            _jwt = jwt.Value;
        }


        //[FromHeader(Name = "Authorization")]
        //public string Token { get; set; }


        #region Actions

        /// <summary>
        /// Registration method
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/register")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterAsync(AuthRequest authRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            #region
            // return Ok(new LoginResponse { Token = string.Empty });

            //and logins back
            //validations, call business login,
            //

            //if (!ModelState.IsValid) return BadRequest(ModelState);

            // business logic: register the account 
            // var account = ...
            // check user is empty -> Conflict
            #endregion

            // TODO: business logic: register and check

            var user = new User()
            {
                Id = 1,
                Email = "omel.omelian.com",
                Login = "skoooby",
                PasswordHash = "hdDGSHDhdhjsuGD",
                Score = 0,
                Role = Roles.Admin
            };

             return await BuildToken(user);
            //return Ok(new LoginResponse { Token = token});
        }

        private class LoginResponse
        {
            public string Token { get; set; }
        }

        [HttpPost]
        [Route("/login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> LoginAsync(AuthRequest authRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // TODO: business logic: is such a user exist?

            var user = new User()
            {
                Id = 1,
                Email = "k.bilotska@gmail.com",
                Login = "lucky_spirit",
                PasswordHash = "hdDGSHDhdhjsuGD",
                Score = 0,
                Role = Roles.Admin
            };

            return await BuildToken(user);
        }
        
        [HttpGet]
        [Route("/logout")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // TODO: Add the parameter
        public Task<HttpStatusCode> LogoutAsync()
        {
            return System.Threading.Tasks.Task.FromResult(HttpStatusCode.OK);
        }
        #endregion

        #region Methods for Jwt
        // We need to decide which fields we will put into claim.
        // It will define which parameter we should pass to BuildToken
        // Now it is User
        private async Task<ActionResult> BuildToken(User user)
        {
            if (user == null) return Unauthorized();
            // we need to get the role of this account
            // var role = 
            //var role = Roles.Admin;

            var identity = GetClaimsIdentity(user.Id, user.Role);

            var now = DateTime.UtcNow;
            var jwtToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(_jwt.LifeTime),
                signingCredentials: new SigningCredentials(_jwt.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // ??
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Ok(response);
        }

        // TODO: Change Claim: add another claim identites
        private static ClaimsIdentity GetClaimsIdentity(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("id", userId.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            var claimsIdentity =
                new ClaimsIdentity(
                    claims,
                    "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
        #endregion
    }
}