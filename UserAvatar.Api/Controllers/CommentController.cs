using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;
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

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateCommentAsync(int boardId, int cardId, CommentDto commentDto)
        {
            var comment = await _commentService.CreateNewCommentAsync(UserId, cardId, commentDto.Text);
            
            return Ok(_mapper.Map<CommentModel,CommentVm>(comment));
        }
        
        [HttpPatch("{commentId:int}")]
        public async Task<IActionResult> UpdateCommentAsync(int boardId, int cardId, int commentId, CommentDto commentDto)
        {
            await _commentService.UpdateCommentAsync(UserId, commentId, commentDto.Text);

            return Ok();
        }
        
        [HttpDelete("{commentId:int}")]
        public async Task<IActionResult> DeleteCommentAsync(int boardId, int columnId, int cardId, int commentId)
        {
            await _commentService.DeleteCommentAsync(UserId,commentId);
            return Ok();
        }
    }
}