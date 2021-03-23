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
    
    [ApiController]
    [Authorize]
    [Route("api/v1/column")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnService _columnService;
        private readonly IMapper _mapper;
        public ColumnController(IColumnService columnService, IMapper mapper)
        {
            _columnService = columnService;
            _mapper = mapper;
        }

        [HttpGet("{boardId:int}")]
        public async Task<ActionResult<List<ColumnDto>>> GetAllColumnsAsync(int boardId)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            var foundColumn = await _columnService.GetAllColumnsAsync(userId,boardId);

            return Ok(_mapper.Map<List<ColumnModel>,List<FullColumnDto>>(foundColumn));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateColumnAsync(CreateColumnRequest createColumnRequest)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            var thisColumn = await _columnService
                .CreateAsync(userId,createColumnRequest.BoardId,createColumnRequest.Title);
            return Ok(_mapper.Map<ColumnModel,FullColumnDto>(thisColumn));
        }
        
        [HttpPatch]
        public async Task<IActionResult> UpdateColumnAsync(UpdateColumnRequest updateColumnRequest)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            await _columnService.UpdateAsync(userId,updateColumnRequest.ColumnId, updateColumnRequest.Title);
            return Ok();
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteColumnAsync([FromQuery]int columnId)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            await _columnService.DeleteAsync(userId,columnId);
            return Ok();
        }
        
        [HttpPost("[controller]/changePosition/")]
        public async Task<IActionResult> ChangeColumnPositionAsync(int columnId, int positionIndex)
        {            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            await _columnService.ChangePositionAsync(userId,columnId,positionIndex);
            return Ok();
        }
    }
}