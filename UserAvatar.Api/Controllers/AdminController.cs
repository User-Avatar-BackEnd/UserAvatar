using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IEventService _eventService;

        public AdminController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("events")]
        [ProducesResponseType(typeof(List<EventModel>),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<List<EventModel>>> GetEvents()
        {
            var events = _eventService.GetEventList();
            return Ok(events);
        }
    }
}
