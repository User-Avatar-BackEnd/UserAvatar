﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Authentication;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers;

/// <summary>
///     Board controller
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/boards")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
public sealed class BoardController : ControllerBase
{
    private readonly IApplicationUser _applicationUser;
    private readonly IBoardChangesService _boardChangesService;
    private readonly IBoardService _boardService;
    private readonly IHistoryService _historyService;
    private readonly IInviteService _inviteService;
    private readonly IMapper _mapper;
    private readonly IRankService _rankService;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="boardService">board service</param>
    /// <param name="inviteService">invite service</param>
    /// <param name="mapper">automapper</param>
    /// <param name="applicationUser">this user id</param>
    /// <param name="historyService">history service</param>
    /// <param name="boardChangesService">tracking changes in background</param>
    /// <param name="rankService">rank service</param>
    public BoardController(
        IBoardService boardService,
        IInviteService inviteService,
        IMapper mapper,
        IApplicationUser applicationUser,
        IHistoryService historyService,
        IBoardChangesService boardChangesService,
        IRankService rankService)
    {
        _boardService = boardService;
        _mapper = mapper;
        _applicationUser = applicationUser;
        _inviteService = inviteService;
        _historyService = historyService;
        _boardChangesService = boardChangesService;
        _rankService = rankService;
    }

    private int UserId => _applicationUser.Id;

    /// <summary>
    ///     Gets all boards
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<BoardShortVm>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<BoardShortVm>>> GetBoardsAsync()
    {
        var result = await _boardService.GetAllBoardsAsync(UserId);

        var models = result.Value.ToList();

        var viewModels = _mapper.Map<List<BoardModel>, List<BoardShortVm>>(models);

        for (var i = 0; i < result.Value.Count(); i++)
        {
            viewModels[i].IsOwner = models[i].OwnerId == UserId;
        }

        return Ok(viewModels);
    }

    /// <summary>
    ///     Creating a board
    /// </summary>
    /// <param name="titleDto">board title</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BoardShortVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<ActionResult<BoardShortVm>> CreateBoardAsync(TitleDto titleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        titleDto.Title = titleDto.Title.Trim();

        var result = await _boardService.CreateBoardAsync(UserId, titleDto.Title);

        if (result.Code != ResultCode.Success)
        {
            return Conflict(result.Code);
        }

        await _historyService.AddEventToHistoryAsync(UserId, result.EventType);

        var board = _mapper.Map<BoardModel, BoardShortVm>(result.Value);
        board.IsOwner = result.Value.OwnerId == UserId;

        return Ok(board);
    }

    /// <summary>
    ///     Gets specific board
    /// </summary>
    /// <param name="boardId">id of board</param>
    /// <returns></returns>
    [HttpGet("{boardId:int}")]
    [ProducesResponseType(typeof(BoardVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<BoardVm>> GetBoardAsync(int boardId)
    {
        var result = await _boardService.GetBoardAsync(UserId, boardId);

        //Here can be switch Case statement. Please remind to change
        if (result.Code == ResultCode.Forbidden)
        {
            return Forbid();
        }

        if (result.Code == ResultCode.NotFound)
        {
            return NotFound();
        }


        var scores = result.Value.Members.Select(member => member.User.Score).ToList();

        var ranks = await _rankService.GetRanksAsync(scores);
        for (var i = 0; i < result.Value.Members.Count; i++)
        {
            result.Value.Members[i].Rank = ranks[i];
        }

        var boardVm = _mapper.Map<BoardModel, BoardVm>(result.Value);

        boardVm.IsOwner = result.Value.OwnerId == UserId;

        return Ok(boardVm);
    }

    /// <summary>
    ///     Renaming a board
    /// </summary>
    /// <param name="boardId">id of board</param>
    /// <param name="titleDto">new board title</param>
    /// <returns></returns>
    [HttpPatch("{boardId:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<IActionResult> RenameBoardAsync(int boardId, TitleDto titleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        titleDto.Title = titleDto.Title.Trim();

        var result = await _boardService.RenameBoardAsync(UserId, boardId, titleDto.Title);

        return StatusCode(result);
    }

    /// <summary>
    ///     Soft deletes board
    /// </summary>
    /// <param name="boardId"></param>
    /// <returns></returns>
    [HttpDelete("{boardId:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteBoardAsync(int boardId)
    {
        var result = await _boardService.DeleteBoardAsync(UserId, boardId);

        if (result == ResultCode.Forbidden)
        {
            return Forbid();
        }

        if (result == ResultCode.NotFound)
        {
            return NotFound();
        }

        return StatusCode(result);
    }

    /// <summary>
    ///     Deletes member from a specific board
    /// </summary>
    /// <param name="boardId">id of this board</param>
    /// <param name="toDeleteUserId">id of user to be deleted</param>
    /// <returns></returns>
    [HttpDelete("{boardId:int}/user/")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteMemberFromBoardAsync(int boardId, [FromQuery] int toDeleteUserId)
    {
        var result = await _boardService.DeleteMemberFromBoardAsync(UserId, toDeleteUserId, boardId);

        if (result == ResultCode.Forbidden)
        {
            return Forbid();
        }

        if (result == ResultCode.NotFound)
        {
            return NotFound();
        }

        return Ok();
    }

    /// <summary>
    ///     Creates an invitation to the board
    /// </summary>
    /// <param name="boardId">id of board to be invited in</param>
    /// <param name="payload">id or email of user to be invited</param>
    /// <returns></returns>
    [HttpPost("{boardId:int}/invites")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<IActionResult> CreateInvitationAsync(
        int boardId,
        [Required(AllowEmptyStrings = false)] string payload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _inviteService.CreateInviteAsync(boardId, UserId, payload);

        if (result.Code == ResultCode.Forbidden)
        {
            return Forbid();
        }

        if (result.Code == ResultCode.NotFound)
        {
            return NotFound();
        }

        await _historyService.AddEventToHistoryAsync(UserId, result.EventType);

        return Ok();
    }

    /// <summary>
    ///     Gets user by search query
    /// </summary>
    /// <param name="boardId">id of this board</param>
    /// <param name="query">search query</param>
    /// <returns></returns>
    [HttpGet("{boardId:int}/invites/find_person")]
    [ProducesResponseType(typeof(List<UserShortVm>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<List<UserShortVm>>> GetUsersByQuery(int boardId, [FromQuery] string query)
    {
        var result = await _inviteService.FindByQueryAsync(boardId, UserId, query);
        switch (result.Code)
        {
            case ResultCode.Forbidden:
                return Forbid();
            case ResultCode.NotFound:
                return NotFound();
        }

        var scores = result.Value.Select(x => x.Score).ToList();
        var ranks = await _rankService.GetRanksAsync(scores);
        var mapped = _mapper.Map<List<UserModel>, List<UserShortVm>>(result.Value);
        for (var i = 0; i < mapped.Count; i++)
        {
            mapped[i].Rank = ranks[i];
        }

        return Ok(mapped);
    }

    /// <summary>
    ///     Method for checking changes on board
    /// </summary>
    /// <param name="boardId">id of this board</param>
    /// <param name="ticks">amount of ticks</param>
    /// <returns></returns>
    [HttpGet("{boardId:int}/changes")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<IActionResult> CheckChanges(int boardId, [FromQuery] long? ticks)
    {
        if (!await _boardService.IsUserBoardAsync(UserId, boardId))
        {
            return Forbid();
        }

        if (ticks == null)
        {
            return Ok(new { DateTimeOffset.UtcNow.Ticks, Changed = false });
        }

        var hasChanges = _boardChangesService.HasChanges(boardId, UserId, (long)ticks);
        return Ok(new { DateTimeOffset.UtcNow.Ticks, Changed = hasChanges });
    }
}
