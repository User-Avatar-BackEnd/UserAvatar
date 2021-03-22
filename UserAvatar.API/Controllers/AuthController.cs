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
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Infrastructure.Exceptions;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtOptions _jwt;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, IOptions<JwtOptions> jwt)
        {
            _authService = authService;
            _jwt = jwt.Value;
        }

        #region Actions

        /// <summary>
        /// Registration method
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = _authService.Register(registerRequest.Email, registerRequest.Login, registerRequest.Password);

                if (user == null) Unauthorized();

                return BuildToken(user);
            }
            catch(InformException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = _authService.Login(loginRequest.Email, loginRequest.Password);

                if (user == null) return Unauthorized();

                return BuildToken(user);
            }
            catch (InformException ex)
            {
                return Conflict(ex.Message);
            }
        }
        
        [HttpGet("logout")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public Task<HttpStatusCode> LogoutAsync()
        {
            // call gamification service to add scores for the logout
            return System.Threading.Tasks.Task.FromResult(HttpStatusCode.OK);
        }
        #endregion

        #region Methods for Jwt
        private ActionResult BuildToken(UserModel user)
        {
            if (user == null) return Unauthorized();

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

            var response = new
            {
                access_token = encodedJwt,
                role = user.Role
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