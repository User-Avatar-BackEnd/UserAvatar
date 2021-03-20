using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.API.Contracts;
using UserAvatar.BLL.Models;
using UserAvatar.BLL.Services;

namespace UserAvatar.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/Board")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IMapper _mapper;
        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<BoardModel, BoardDto>()).CreateMapper();
        }

        [HttpGet]
        public async Task<ActionResult<List<BoardDto>>> GetBoards()
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var result = await _boardService.GetAllBoards(userId);

            // what?
            _mapper.Map<IEnumerable<BoardModel>, IEnumerable<BoardDto>>(result);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<object>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateBoard(BoardRequest boardRequest)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

          var success = await _boardService.CreateBoard(userId, boardRequest.Title);

            if (!success) return BadRequest();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetBoard([FromHeader] int boardId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var board = await _boardService.GetBoard(userId, boardId);

            return Ok(_mapper.Map<BoardModel, BoardDto>(board));
        }

        [HttpPatch]
        public async Task<IActionResult> RenameBoard([FromHeader] BoardRequest boardRequest)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var success = await _boardService.RenameBoard(userId, boardRequest.Id, boardRequest.Title);

            if (!success) return BadRequest();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBoard([FromHeader] int boardId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

           var success = await _boardService.DeleteBoard(userId, boardId);

            if (!success) return BadRequest();

            return Ok();
        }
    }
}