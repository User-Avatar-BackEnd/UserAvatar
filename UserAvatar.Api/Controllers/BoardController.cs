using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Options;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Bll.TaskManager;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using UserAvatar.Bll.Gamification.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/boards")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _applicationUser;
        private readonly IInviteService _inviteService;
        private readonly IHistoryService _historyService;

        public BoardController(
            IBoardService boardService,
            IInviteService inviteService,
            IMapper mapper,
            IApplicationUser applicationUser,
            IHistoryService historyService)
        {
            _boardService = boardService;
            _mapper = mapper;
            _applicationUser = applicationUser;
            _inviteService = inviteService;
            _historyService = historyService;
        }

        private int UserId => _applicationUser.Id;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BoardShortVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<BoardVm>>> GetBoardsAsync()
        {
            var result = await _boardService.GetAllBoardsAsync(UserId);

            var list =
                _mapper.Map<IEnumerable<BoardModel>,
                    IEnumerable<BoardShortVm>>(result.Value);

            return Ok(list);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BoardShortVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<BoardShortVm>> CreateBoardAsync(TitleDto titleDto)
        {
            titleDto.Title = titleDto.Title.Trim();

            var result =
                await _boardService.CreateBoardAsync(UserId, titleDto.Title);

            if (result.Code != ResultCode.Success)
            {
                return Conflict(result.Code);
            }

            await _historyService.AddEventToHistoryAsync(UserId, result.EventType);

            return Ok(_mapper.Map<BoardModel, BoardShortVm>(result.Value));
        }

        [HttpGet("{boardId:int}")]
        [ProducesResponseType(typeof(BoardVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult<BoardVm>> GetBoardAsync(int boardId)
        {
            var result = await _boardService.GetBoardAsync(UserId, boardId);

            //Here can be switch Case statement. Please remind to change
            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();

            var boardVm = _mapper.Map<BoardModel, BoardVm>(result.Value);

            boardVm.IsOwner = result.Value.OwnerId == UserId;

            return Ok(boardVm);
        }

        [HttpPatch("{boardId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> RenameBoardAsync(int boardId, TitleDto titleDto)
        {
            titleDto.Title = titleDto.Title.Trim();

            var result =
                await _boardService
                    .RenameBoardAsync(UserId, boardId, titleDto.Title);

            return StatusCode(result);
        }

        [HttpDelete("{boardId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> DeleteBoardAsync(int boardId)
        {
            var result = await _boardService.DeleteBoardAsync(UserId, boardId);

            if (result == ResultCode.Forbidden) return Forbid();
            if (result == ResultCode.NotFound) return NotFound();

            return StatusCode(result);
        }

        [HttpPost("{boardId:int}/invites")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> CreateInvitationAsync(
            int boardId,
            [Required(AllowEmptyStrings = false)] string payload)
        {
            var result = await _inviteService.CreateInviteAsync(boardId, UserId, payload);

            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();

            await _historyService.AddEventToHistoryAsync(UserId, result.EventType);

            return Ok();
        }

        [HttpGet("{boardId:int}/invites/find_person")]
        [ProducesResponseType(typeof(List<UserShortVm>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult<List<UserShortVm>>> GetUsersByQuery(int boardId, [FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return NotFound();

            var result = await _inviteService.FindByQuery(boardId, UserId, query);

            return result.Code switch
            {
                ResultCode.Forbidden => Forbid(),
                ResultCode.NotFound => NotFound(),
                _ => Ok(_mapper.Map<List<UserModel>, List<UserShortVm>>(result.Value))
            };
        }
    }
}