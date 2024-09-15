using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Authentication;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers;

/// <summary>
///     Commentaries controller
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/boards/{boardId:int}/cards/{cardId:int}/comments")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public sealed class CommentController : ControllerBase
{
    private readonly IApplicationUser _applicationUser;
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="commentService">commentaries service</param>
    /// <param name="mapper">automapper</param>
    /// <param name="applicationUser">this user id</param>
    public CommentController(ICommentService commentService, IMapper mapper, IApplicationUser applicationUser)
    {
        _commentService = commentService;
        _mapper = mapper;
        _applicationUser = applicationUser;
    }

    private int UserId => _applicationUser.Id;

    /// <summary>
    ///     Creates comment in a card
    /// </summary>
    /// <param name="boardId">board id where card is</param>
    /// <param name="cardId">card to be inserted in id</param>
    /// <param name="commentDto">comment data</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CommentVm), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<CommentDto>> CreateCommentAsync(int boardId, int cardId, CommentDto commentDto)
    {
        var result = await _commentService.CreateNewCommentAsync(UserId, boardId, cardId, commentDto.Text);

        if (result.Code == ResultCode.NotFound)
        {
            return NotFound();
        }

        if (result.Code == ResultCode.Forbidden)
        {
            return Forbid();
        }

        return Ok(_mapper.Map<CommentModel, CommentVm>(result.Value));
    }

    /// <summary>
    ///     Updates comment
    /// </summary>
    /// <param name="boardId">board id where card is</param>
    /// <param name="cardId">card where comment is id</param>
    /// <param name="commentId">comment to be updated id</param>
    /// <param name="commentDto">update comment body</param>
    /// <returns></returns>
    [HttpPatch("{commentId:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> UpdateCommentAsync(int boardId, int cardId, int commentId, CommentDto commentDto)
    {
        var result = await _commentService.UpdateCommentAsync(UserId, boardId, cardId, commentId, commentDto.Text);

        if (result.Code == ResultCode.NotFound)
        {
            return NotFound();
        }

        if (result.Code == ResultCode.Forbidden)
        {
            return Forbid();
        }

        return Ok(_mapper.Map<CommentModel, CommentVm>(result.Value));
    }

    /// <summary>
    ///     Soft deletes comment
    /// </summary>
    /// <param name="boardId">board id where card with comment is</param>
    /// <param name="cardId">card id where comment is</param>
    /// <param name="commentId">comment id to be deleted</param>
    /// <returns></returns>
    [HttpDelete("{commentId:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> DeleteCommentAsync(int boardId, int cardId, int commentId)
    {
        var result = await _commentService.DeleteCommentAsync(UserId, boardId, cardId, commentId);

        if (result == ResultCode.NotFound)
        {
            return NotFound();
        }

        if (result == ResultCode.Forbidden)
        {
            return Forbid();
        }

        return StatusCode(result);
    }
}
