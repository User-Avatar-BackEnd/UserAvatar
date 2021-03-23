using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Options;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Bll.TaskManager.Infrastructure;
using UserAvatar.Api.Contracts.ViewModels;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/boards")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _applicationUser;
        public BoardController(IBoardService boardService, IMapper mapper, IApplicationUser applicationUser)
        {
            _boardService = boardService;
            _mapper = mapper;
            _applicationUser = applicationUser;
        }

        private int UserId => _applicationUser.Id;

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<List<BoardVm>>> GetBoardsAsync()
        {
            var result = await _boardService.GetAllBoardsAsync(UserId);

            var list = _mapper.Map<IEnumerable<BoardModel>, IEnumerable<BoardShortVm>>(result.Value);

            return Ok(list);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateBoardAsync(TitleDto titleDto)
        {
            titleDto.Title = titleDto.Title.Trim();

            int code = await _boardService.CreateBoardAsync(UserId, titleDto.Title);

            if (code != ResultCode.Success)
            {
                return Conflict(code);
            }

            return Ok();
        }

        [HttpGet("{boardId:int}")]
        [ProducesResponseType(typeof(BoardVm),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<BoardVm>> GetBoardAsync(int boardId)
        {
            var result = await _boardService.GetBoardAsync(UserId, boardId);

            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();

            var boardVm = _mapper.Map<BoardModel, BoardVm>(result.Value);

            boardVm.IsOwner = result.Value.OwnerId == userId;


            return Ok(boardVm);
        }

        [HttpPatch("{boardId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> RenameBoardAsync(int boardId, TitleDto titleDto)
        {
            titleDto.Title = titleDto.Title.Trim();

            int result = await _boardService.RenameBoardAsync(UserId, boardId, titleDto.Title);

            return StatusCode(result);
        }

        [HttpDelete("{boardId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteBoardAsync(int boardId)
        {
            int result = await _boardService.DeleteBoardAsync(UserId, boardId);

            return StatusCode(result);
        }
    }
}