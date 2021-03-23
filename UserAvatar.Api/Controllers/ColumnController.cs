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

    [ApiController]
    [Authorize]
    [Route("api/v1/boards/{boardId:int}/columns")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnService _columnService;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _applicationUser;
        public ColumnController(IColumnService columnService, IMapper mapper, IApplicationUser applicationUser)
        {
            _columnService = columnService;
            _mapper = mapper;
            _applicationUser = applicationUser;
        }

        private int UserId => _applicationUser.Id;

        /*
        [HttpGet("{boardId:int}")]
        public async Task<ActionResult<List<ColumnVm>>> GetAllColumnsAsync(int boardId)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            var foundColumn = await _columnService.GetAllColumnsAsync(userId,boardId);

            return Ok(_mapper.Map<List<ColumnModel>,List<FullColumnVm>>(foundColumn));
        }
        */

        [HttpPost]
        public async Task<IActionResult> CreateColumnAsync(int boardId, TitleDto titleDto)
        {
            titleDto.Title = titleDto.Title.Trim();
            var thisColumn = await _columnService.CreateAsync(UserId, boardId, titleDto.Title);
            return Ok(_mapper.Map<ColumnModel, FullColumnVm>(thisColumn));
        }

        [HttpPatch("{columnId:int}")]
        public async Task<IActionResult> UpdateColumnAsync(int boardId, int columnId, TitleDto titleDto)
        {
            titleDto.Title = titleDto.Title.Trim();
            await _columnService.UpdateAsync(UserId,columnId, titleDto.Title);
            return Ok();
        }
        
        [HttpDelete("{columnId:int}")]
        public async Task<IActionResult> DeleteColumnAsync(int boardId, int columnId)
        {
            await _columnService.DeleteAsync(UserId, columnId);
            return Ok();
        }
        
        [HttpPost("{columnId:int}/position")]
        public async Task<IActionResult> ChangeColumnPositionAsync(int boardId, int columnId, [FromQuery]int to)
        {   
            await _columnService.ChangePositionAsync(UserId,columnId,to);
            return Ok();
        }
    }
}