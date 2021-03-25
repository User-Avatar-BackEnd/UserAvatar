using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/boards/{boardId:int}/cards/{cardId:int}/comments")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class CommentController:ControllerBase
    {

        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _applicationUser;
        public CommentController(ICommentService commentService, IMapper mapper, IApplicationUser applicationUser)
        {
            _commentService = commentService;
            _mapper = mapper;
            _applicationUser = applicationUser;
        }

        private int UserId => _applicationUser.Id;

        /*
        [HttpGet]
        public async Task<ActionResult<CommentDto>> GetCommentsAsync(int cardId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);
            var list = await _commentService.GetCommentsAsync(userId,cardId);

            if (list.Code != ResultCode.Success)
            {
                return Forbid();
            }
            return Ok(_mapper.Map<List<CommentModel>,List<CommentDto>>(list.Value));



            return Ok(_mapper.Map<List<CommentModel>,List<CommentDto>>(list));
        }
        */

        [HttpPost]
        [ProducesResponseType(typeof(CommentVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<CommentDto>> CreateCommentAsync(int boardId, int cardId, CommentDto commentDto)
        {
            var result = await _commentService.CreateNewCommentAsync(UserId, boardId, cardId, commentDto.Text);

            if (result.Code == ResultCode.NotFound) return NotFound();
            if (result.Code == ResultCode.Forbidden) return Forbid();
            
            return Ok(_mapper.Map<CommentModel,CommentVm>(result.Value));
        }
        
        [HttpPatch("{commentId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> UpdateCommentAsync(int boardId, int cardId, int commentId, CommentDto commentDto)
        {
            var result = await _commentService.UpdateCommentAsync(UserId, boardId, cardId, commentId, commentDto.Text);

            if (result.Code == ResultCode.NotFound) return NotFound();
            if (result.Code == ResultCode.Forbidden) return Forbid();

            return Ok(_mapper.Map<CommentModel, CommentVm>(result.Value));
        }
        
        [HttpDelete("{commentId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> DeleteCommentAsync(int boardId, int cardId, int commentId)
        {
            var result = await _commentService.DeleteCommentAsync(UserId, boardId, cardId, commentId);

            if (result == ResultCode.NotFound) return NotFound();
            if (result == ResultCode.Forbidden) return Forbid();

            return StatusCode(result);
        }
    }
}