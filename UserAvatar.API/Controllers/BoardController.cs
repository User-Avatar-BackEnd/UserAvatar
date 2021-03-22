using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/Board")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IMapper _mapper;
        public BoardController(IBoardService boardService, IMapper mapper)
        {
            _boardService = boardService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<BoardDto>>> GetBoardsAsync()
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var result = await _boardService.GetAllBoardsAsync(userId);

            var list = _mapper.Map<IEnumerable<BoardModel>, IEnumerable<BoardDto>>(result);

            return Ok(list);
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<object>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateBoardAsync(BoardRequest boardRequest)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            await _boardService.CreateBoardAsync(userId, boardRequest.Title);

            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBoardAsync(int id)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var board = await _boardService.GetBoardAsync(userId, id);

            return Ok(_mapper.Map<BoardModel, BoardDto>(board));
        }

        [HttpPatch]
        public async Task<IActionResult> RenameBoardAsync([FromBody] BoardRequest boardRequest)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            await _boardService.RenameBoardAsync(userId, boardRequest.Id, boardRequest.Title);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBoardAsync(int id)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            await _boardService.DeleteBoardAsync(userId, id);

            return Ok();
        }
    }
}