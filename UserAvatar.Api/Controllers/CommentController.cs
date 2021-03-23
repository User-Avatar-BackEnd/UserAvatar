using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
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
        public async Task<ActionResult<CommentDto>> GetCommentsAsync(int cardId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);
            var list = await _commentService.GetCommentsAsync(userId,cardId);

            return Ok(_mapper.Map<List<CommentModel>,List<CommentDto>>(list));

        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateCommentAsync(CommentDto commentDto)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);
            var comment = await _commentService.CreateNewCommentAsync(userId, commentDto.CardId, commentDto.Text);
            
            return Ok(_mapper.Map<CommentModel,CommentDto>(comment));
        }
        
        [HttpPatch]
        public async Task<IActionResult> UpdateCommentAsync(UpdateCommentDto updateCommentDto)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);
            await _commentService.UpdateCommentAsync(userId, updateCommentDto.CommentId, updateCommentDto.Text);

            return Ok();
        }
        
        [HttpDelete()]
        public async Task<IActionResult> DeleteCommentAsync([FromQuery]int columnId)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            await _commentService.DeleteCommentAsync(userId,columnId);
            return Ok();
        }
    }
}