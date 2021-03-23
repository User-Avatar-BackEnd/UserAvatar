using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/Comments")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class CommentController:ControllerBase
    {

        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CommentDto>> GetComments(int cardId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);
            var list = await _commentService.GetComments(userId,cardId);

            return Ok(_mapper.Map<List<CommentModel>,List<CommentDto>>(list));

        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CommentRequest commentRequest)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);
            var comment = await _commentService.CreateNewCommentAsync(userId, commentRequest.CardId, commentRequest.Text);
            
            return Ok(_mapper.Map<CommentModel,CommentDto>(comment));
        }
        
        [HttpPatch]
        public async Task<IActionResult> UpdateComment(UpdateCommentRequest updateCommentRequest)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);
            await _commentService.UpdateComment(userId, updateCommentRequest.CommentId, updateCommentRequest.Text);

            return Ok();
        }
        
        [HttpDelete()]
        public async Task<IActionResult> DeleteComment([FromQuery]int columnId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            await _commentService.DeleteComment(userId,columnId);
            return Ok();
        }
    }
}