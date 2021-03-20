using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<BoardModel>>> GetBoards()
        {
            //todo: remove DRY
            //if (!ModelState.IsValid) return BadRequest();
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var id = Convert.ToInt32(userCredentials.Value);

            var result = _boardService.GetAllBoardsById(id);
            
            
            //Needed fields to list boards
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<object>),200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CreateBoard(BoardRequest boardRequest)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var id = Convert.ToInt32(userCredentials.Value);

            return Ok(_boardService.CreateBoard(id, boardRequest.Title));
        }
        
        
    }
}