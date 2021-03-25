﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;

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
        public async Task<IActionResult> ChangeEventsCostAsync(List<EventDto> newEvents)
        {
            var eventModels = _mapper.Map<List<EventDto>, List<EventModel>>(newEvents);

            int result = await _eventService.ChangeEventsCostAsync(eventModels);

            if (result == ResultCode.BadRequest) return BadRequest();

            return Ok();
        }

        [HttpGet("history/{login:string}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<HistoryVm>> GetHistory(string login)
        {
            var result = await _eventService.GetHistoryAsync(login);

            if (result.Code == ResultCode.NotFound) return NotFound();

            return Ok(_mapper.Map<List<HistoryModel>, List<HistoryVm>>(result.Value));
        }

    }
}
