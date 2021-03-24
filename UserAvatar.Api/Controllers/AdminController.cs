using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public class AdminController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public AdminController(IEventService eventService, IMapper mapper)
        {
            _eventService = eventService;
            _mapper = mapper;
        }

        [HttpGet("events")]
        [ProducesResponseType(typeof(List<EventVm>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<EventVm>>> GetEventsAsync()
        {
            var events = await _eventService.GetEventListAsync();
            return Ok(_mapper.Map<List<EventModel>, List<EventVm>>(events));
        }

        [HttpPut("events")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<EventVm>>> ChangeEventsCostAsync(List<EventDto> oldEvents)
        {
            throw new NotImplementedException();
        }
    }
}
