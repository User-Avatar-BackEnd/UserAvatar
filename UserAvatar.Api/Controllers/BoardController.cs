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
    [Route("api/v1/board")]
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

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<List<BoardVm>>> GetBoardsAsync()
        {
            var userId = _applicationUser.Id;

            var result = await _boardService.GetAllBoardsAsync(userId);

            var list = _mapper.Map<IEnumerable<BoardModel>, IEnumerable<BoardShortVm>>(result.Value);

            return Ok(list);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateBoardAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest("Empty title");
            title = title.Trim();

            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            int code = await _boardService.CreateBoardAsync(userId, title);

            if (code != ResultCode.Success)
            {
                return Conflict(code);
            }

            return Ok();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BoardVm),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<BoardVm>> GetBoardAsync(int id)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var result = await _boardService.GetBoardAsync(userId, id);

            if (result.Code == ResultCode.Forbidden) return Forbid();
            if (result.Code == ResultCode.NotFound) return NotFound();

            var boardVm = _mapper.Map<BoardModel, BoardVm>(result.Value);

            boardVm.IsOwner = result.Value.User.Id == userId;

            return Ok(boardVm);
        }

        [HttpPatch]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> RenameBoardAsync(UpdateBoardDto boardDto)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            int result = await _boardService.RenameBoardAsync(userId, boardDto.Id, boardDto.Title);

            return StatusCode(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteBoardAsync(int id)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            int result = await _boardService.DeleteBoardAsync(userId, id);

            return StatusCode(result);
        }
    }
}