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
        
        [HttpGet]
        public async Task<ActionResult<List<FullColumnDto>>> GetAllColumns([FromQuery] int boardId)
        {
            var foundColumn = await _columnService.GetAllColumns(boardId);

            return Ok(_mapper.Map<List<ColumnModel>,List<FullColumnDto>>(foundColumn));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateColumn(CreateColumnRequest createColumnRequest)
        {
            var thisColumn = await _columnService
                .Create(createColumnRequest.BoardId,createColumnRequest.Title);
            return Ok(_mapper.Map<ColumnModel,FullColumnDto>(thisColumn));
        }
        
        [HttpPatch]
        public async Task<IActionResult> UpdateColumn(UpdateColumnRequest updateColumnRequest)
        {
            await _columnService.Update(updateColumnRequest.ColumnId, updateColumnRequest.Title);
            return Ok();
        }
        
        [HttpDelete("{columnId:int}")]
        public async Task<IActionResult> DeleteColumn([FromQuery]int columnId)
        {
            await _columnService.Delete(columnId);
            return Ok();
        }
        
        [HttpGet("{columnId:int}&{positionIndex:int}")]
        public async Task<IActionResult> ChangeColumnPosition([FromQuery] int columnId,
            [FromQuery] int positionIndex)
        {
            await _columnService.ChangePosition(columnId,positionIndex);
            return Ok();
        }
    }
}