using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.API.Contracts.Dtos;
using UserAvatar.API.Contracts.Requests;
using UserAvatar.BLL.Models;
using UserAvatar.BLL.Services.Interfaces;

namespace UserAvatar.API.Controllers
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
        [Route("/get_boards")]
        public async Task<ActionResult<List<BoardDto>>> GetBoardsAsync()
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var result = await _boardService.GetAllBoardsAsync(userId);

            // what?
            _mapper.Map<IEnumerable<BoardModel>, IEnumerable<BoardDto>>(result);

            return Ok(result);
        }

        [HttpPost]
        [Route("/create_board")]
        [ProducesResponseType(typeof(List<object>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateBoardAsync(BoardRequest boardRequest)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

          var success = await _boardService.CreateBoardAsync(userId, boardRequest.Title);

            if (!success) return BadRequest();

            return Ok();
        }

        [HttpGet]
        [Route("/get_board")]
        public async Task<IActionResult> GetBoardAsync([FromHeader] int boardId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var board = await _boardService.GetBoardAsync(userId, boardId);

            return Ok(_mapper.Map<BoardModel, BoardDto>(board));
        }

        // problem: doesn't set up a board id
        [HttpPatch]
        [Route("/rename_board")]
        public async Task<IActionResult> RenameBoardAsync([FromBody] BoardRequest boardRequest)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var success = await _boardService.RenameBoardAsync(userId, boardRequest.Id, boardRequest.Title);

            if (!success) return BadRequest();

            return Ok();
        }

        [HttpDelete]
        [Route("/delete_board")]
        public async Task<IActionResult> DeleteBoardAsync([FromBody] int boardId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

           var success = await _boardService.DeleteBoardAsync(userId, boardId);

            if (!success) return BadRequest();

            return Ok();
        }
    }
}