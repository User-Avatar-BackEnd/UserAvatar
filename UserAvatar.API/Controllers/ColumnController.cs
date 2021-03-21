using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.API.Contracts.Requests;
using UserAvatar.BLL.Services.Interfaces;

namespace UserAvatar.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/columns")]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnService _columnService;
        public ColumnController(IColumnService columnService)
        {
            _columnService = columnService;
        }

        [HttpPost]
        [Route("/create")]
        public IActionResult CreateColumn(ColumnRequest columnRequest)
        {
            _columnService
                .Create(columnRequest.BoardOrColumnId,columnRequest.Title);
            return Ok();
        }
        
        [HttpPatch]
        [Route("/modify")]
        public IActionResult UpdateColumn(ColumnRequest columnRequest)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete]
        [Route("/delete")]
        public IActionResult DeleteColumn(ColumnRequest columnRequest)
        {
            _columnService.Delete(columnRequest.BoardOrColumnId);
            return Ok();
        }
        
        [HttpGet]
        [Route("/change_position/")]
        public IActionResult ChangeColumnPosition([FromHeader] int columnId,
            [FromQuery] int positionIndex)
        {
            _columnService.ChangePosition(columnId,positionIndex);
            return Ok();
        }
    }
}