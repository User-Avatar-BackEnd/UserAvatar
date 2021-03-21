using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Bll.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/")]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnService _columnService;
        public ColumnController(IColumnService columnService)
        {
            _columnService = columnService;
        }

        [HttpPost]
        [Route("[controller]/create")]
        public IActionResult CreateColumn(ColumnRequest columnRequest)
        {
            _columnService
                .Create(columnRequest.BoardOrColumnId,columnRequest.Title);
            return Ok();
        }
        
        [HttpPatch]
        [Route("[controller]/modify")]
        public IActionResult UpdateColumn(ColumnRequest columnRequest)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete]
        [Route("[controller]/delete")]
        public IActionResult DeleteColumn(ColumnRequest columnRequest)
        {
            _columnService.Delete(columnRequest.BoardOrColumnId);
            return Ok();
        }
        
        [HttpGet]
        [Route("[controller]/change_position/")]
        public IActionResult ChangeColumnPosition([FromHeader] int columnId,
            [FromQuery] int positionIndex)
        {
            _columnService.ChangePosition(columnId,positionIndex);
            return Ok();
        }
    }
}