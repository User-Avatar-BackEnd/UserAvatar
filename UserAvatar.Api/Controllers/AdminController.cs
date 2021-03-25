﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AdminController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IPersonalAccountService _personalAccountService;
        private readonly IApplicationUser _applicationUser;
        private readonly IMapper _mapper;

        public AdminController(IEventService eventService,
            IPersonalAccountService _personalAccountService,
            IApplicationUser applicationUser,
            IMapper mapper)
        {
            _eventService = eventService;
            _applicationUser = applicationUser;
            _mapper = mapper;
        }

        private int UserId => _applicationUser.Id;

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
        public async Task<IActionResult> ChangeEventsCostAsync(List<EventDto> newEvents)
        {
            var eventModels = _mapper.Map<List<EventDto>, List<EventModel>>(newEvents);

            int result = await _eventService.ChangeEventsCostAsync(eventModels);

            if (result == ResultCode.BadRequest) return BadRequest();

            return Ok();
        }

        [HttpPut("change_role")]
        public async Task<IActionResult> ChangeRole(ChangeRoleRequest changeRoleRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (changeRoleRequest.Role != Roles.Admin && changeRoleRequest.Role != Roles.User)
            {
                return BadRequest();
            }

            var result = await _personalAccountService.ChangeRole(changeRoleRequest.Id, changeRoleRequest.Role);

            if(result == ResultCode.NotFound) return NotFound();
            if(result == ResultCode.Forbidden) return Forbid();

            return StatusCode(result);
        }
    }
}
