﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserAvatar.Api.Authentication;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers;

/// <summary>
///     Authentication controller
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
public sealed class AuthController : ControllerBase
{
    private readonly IApplicationUser _applicationUser;
    private readonly IAuthService _authService;
    private readonly IHistoryService _historyService;
    private readonly JwtOptions _jwt;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="authService">authentication service</param>
    /// <param name="jwt">jwt token</param>
    /// <param name="historyService">history service</param>
    /// <param name="applicationUser">this user id</param>
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
    ///     Registration method
    /// </summary>
    /// <param name="registerRequest"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> RegisterAsync(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        registerRequest.Email = registerRequest.Email.ToLower();

        var result =
            await _authService.RegisterAsync(registerRequest.Email, registerRequest.Login, registerRequest.Password);

        if (result.Code != ResultCode.Success)
        {
            return Conflict(result.Code);
        }

        await _historyService.AddEventToHistoryAsync(result.Value.Id, result.EventType);

        return BuildToken(result.Value);
    }

    /// <summary>
    ///     Signing in user
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> LoginAsync(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        loginRequest.Email = loginRequest.Email.ToLower();

        var result = await _authService.LoginAsync(loginRequest.Email, loginRequest.Password);

        if (result.Code != ResultCode.Success)
        {
            return Conflict(result.Code);
        }

        await _historyService.AddEventToHistoryAsync(result.Value.Id, result.EventType);

        return BuildToken(result.Value);
    }

    /// <summary>
    ///     Signing out of this user
    /// </summary>
    /// <returns></returns>
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
            _jwt.Issuer,
            _jwt.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(_jwt.LifeTime),
            signingCredentials: new SigningCredentials(_jwt.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return Ok(encodedJwt);
    }

    private static ClaimsIdentity GetClaimsIdentity(int userId, string role)
    {
        var claims = new List<Claim> { new("id", userId.ToString()), new(ClaimsIdentity.DefaultRoleClaimType, role) };
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
