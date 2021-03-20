using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.API.Contracts;

namespace UserAvatar.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/columns")]
    public class ColumnController : ControllerBase
    {
        public ColumnController()
        {
            
        }

        [HttpPost]
        [Route("/create")]
        public IActionResult CreateColumn(ColumnRequest columnRequest)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        
        [HttpGet]
        [Route("/change_position/")]
        public IActionResult ChangeColumnPosition([FromHeader] int columnId,
            [FromQuery] int positionIndex)
        {
            throw new NotImplementedException();
        }
    }
}