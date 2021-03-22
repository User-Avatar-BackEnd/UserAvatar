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
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/Columns")]
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
        public async Task<ActionResult<List<FullColumnDto>>> GetAllColumns(int boardId)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            var foundColumn = await _columnService.GetAllColumns(userId,boardId);

            return Ok(_mapper.Map<List<ColumnModel>,List<FullColumnDto>>(foundColumn));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateColumn(CreateColumnRequest createColumnRequest)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            var thisColumn = await _columnService
                .Create(userId,createColumnRequest.BoardId,createColumnRequest.Title);
            return Ok(_mapper.Map<ColumnModel,FullColumnDto>(thisColumn));
        }
        
        [HttpPatch]
        public async Task<IActionResult> UpdateColumn(UpdateColumnRequest updateColumnRequest)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            await _columnService.Update(userId,updateColumnRequest.ColumnId, updateColumnRequest.Title);
            return Ok();
        }
        
        [HttpDelete("{columnId:int}")]
        public async Task<IActionResult> DeleteColumn(int columnId)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            await _columnService.Delete(userId,columnId);
            return Ok();
        }
        
        [HttpGet("{columnId:int}&{positionIndex:int}")]
        public async Task<IActionResult> ChangeColumnPosition(int columnId,
            int positionIndex)
        {            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);
            
            await _columnService.ChangePosition(userId,columnId,positionIndex);
            return Ok();
        }
    }
}