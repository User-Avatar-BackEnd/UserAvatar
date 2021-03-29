using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Authentication;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{

    /// <summary>
    /// Column Controller
    /// </summary>
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
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columnService">column service</param>
        /// <param name="mapper">automapper</param>
        /// <param name="applicationUser">this user id</param>
        public ColumnController(IColumnService columnService, IMapper mapper, IApplicationUser applicationUser)
        {
            _columnService = columnService;
            _mapper = mapper;
            _applicationUser = applicationUser;
        }

        private int UserId => _applicationUser.Id;
        
        /// <summary>
        /// Creates a column in a board
        /// </summary>
        /// <param name="boardId">board id where to create column</param>
        /// <param name="titleDto">column title</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(FullColumnVm),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> CreateColumnAsync(int boardId, TitleDto titleDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            titleDto.Title = titleDto.Title.Trim();

            var thisColumn = await _columnService.CreateAsync(UserId, boardId, titleDto.Title);

            //Switch case 
            if (thisColumn.Code == ResultCode.NotFound) return NotFound();
            if (thisColumn.Code == ResultCode.Forbidden) return Forbid();

            return Ok(_mapper.Map<ColumnModel, FullColumnVm>(thisColumn.Value));
        }

        /// <summary>
        /// Updates column
        /// </summary>
        /// <param name="boardId">board id where column is</param>
        /// <param name="columnId">this column id</param>
        /// <param name="titleDto">update title</param>
        /// <returns></returns>
        [HttpPatch("{columnId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> UpdateColumnAsync(int boardId, int columnId, TitleDto titleDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            titleDto.Title = titleDto.Title.Trim();

            var result = await _columnService.UpdateAsync(UserId, boardId, columnId, titleDto.Title);

            //Switch case 
            if (result == ResultCode.NotFound) return NotFound();
            if (result == ResultCode.Forbidden) return Forbid();

            return StatusCode(result);
        }

        /// <summary>
        /// Soft deletes column
        /// </summary>
        /// <param name="boardId">board id where column is situated</param>
        /// <param name="columnId">column to be deleted id</param>
        /// <returns></returns>
        [HttpDelete("{columnId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> DeleteColumnAsync(int boardId, int columnId)
        {
            var result = await _columnService.DeleteAsync(UserId, boardId, columnId);

            //Switch case 
            if (result == ResultCode.NotFound) return NotFound();
            if (result == ResultCode.Forbidden) return Forbid();

            return StatusCode(result);
        }

        [HttpPatch("{columnId:int}/position")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> ChangeColumnPositionAsync(int boardId, int columnId, [FromQuery] int? to)
        {
            if (to == null) return BadRequest();
            var result = await _columnService.ChangePositionAsync(UserId, boardId, columnId, (int)to);

            //Switch case 
            if (result == ResultCode.NotFound) return NotFound();
            if (result == ResultCode.Forbidden) return Forbid();

            return StatusCode(result);
        }
    }
}