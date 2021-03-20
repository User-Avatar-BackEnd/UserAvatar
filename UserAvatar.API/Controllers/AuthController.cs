﻿using Microsoft.AspNetCore.Authorization;
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
using UserAvatar.BLL.DTOs;
using UserAvatar.BLL.Services;

namespace UserAvatar.API.Controllers
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
        /// <param name="authRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("/register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterAsync(AuthRequest authRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _authService.Register(authRequest.Email, authRequest.Password);

             return await BuildToken(user);
        }

        [HttpPost]
        [Route("/login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> LoginAsync(AuthRequest authRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _authService.Login(authRequest.Email, authRequest.Password);

            if (user == null) return Unauthorized();

            return await BuildToken(user);
        }
        
        [HttpGet]
        [Route("/logout")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public Task<HttpStatusCode> LogoutAsync()
        {
            // call gamification service to add scores for the logout
            return System.Threading.Tasks.Task.FromResult(HttpStatusCode.OK);
        }
        #endregion

        #region Methods for Jwt
        private async Task<ActionResult> BuildToken(UserDto user)
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