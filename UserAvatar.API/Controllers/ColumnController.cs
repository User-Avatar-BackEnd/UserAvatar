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
        public IActionResult CreateColumn(CreateColumnRequest createColumnRequest)
        {
            _columnService
                .Create(createColumnRequest.Id,createColumnRequest.Title);
            return Ok();
        }
        
        [HttpPatch]
        [Route("[controller]/modify")]
        public IActionResult UpdateColumn(CreateColumnRequest createColumnRequest)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete]
        [Route("[controller]/delete/")]
        public IActionResult DeleteColumn([FromQuery]int columnId)
        {
            _columnService.Delete(columnId);
            return Ok();
        }
        
        [HttpGet]
        [Route("[controller]/change_position/")]
        public IActionResult ChangeColumnPosition([FromQuery] int columnId,
            [FromQuery] int positionIndex)
        {
            _columnService.ChangePosition(columnId,positionIndex);
            return Ok();
        }
    }
}