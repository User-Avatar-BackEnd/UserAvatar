using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public class AuthController : ControllerBase
    {
        private readonly JwtOptions _jwt;
        private readonly IAuthService _authService;
        private readonly IHistoryService _historyService;
        private readonly IApplicationUser _applicationUser;

        public AuthController(
            IAuthService authService,
            IOptions<JwtOptions> jwt,
            IHistoryService historyService,
            IApplicationUser applicationUser)
        {
            _authService = authService;
            _jwt = jwt.Value;
            _historyService = historyService;
            _applicationUser = applicationUser;
        }

        #region Actionspi

        /// <summary>
        /// Registration method
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> RegisterAsync(RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            registerRequest.Email = registerRequest.Email.ToLower();

            var result = await _authService.RegisterAsync(registerRequest.Email, registerRequest.Login, registerRequest.Password);

            if (result.Code != ResultCode.Success)
            {
                return Conflict(result.Code);
            }

            await _historyService.AddEventToHistoryAsync(result.Value.Id, result.EventType);

            return BuildToken(result.Value);
        }

        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> LoginAsync(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            loginRequest.Email = loginRequest.Email.ToLower();

            var result = await _authService.LoginAsync(loginRequest.Email, loginRequest.Password);

            if (result.Code != ResultCode.Success)
            {
                return Conflict(result.Code);
            }

            await _historyService.AddEventToHistoryAsync(result.Value.Id, result.EventType);

            return BuildToken(result.Value);
        }
        
        [HttpGet("logout")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = _applicationUser.Id;

            await _historyService.AddEventToHistoryAsync(userId, _authService.Logout());

            return Ok();
        }
        #endregion

        #region Methods for Jwt
        private ActionResult BuildToken(UserModel user)
        {
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

            return Ok(encodedJwt);
        }

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