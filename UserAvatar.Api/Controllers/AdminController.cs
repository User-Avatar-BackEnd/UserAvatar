using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        private readonly IHistoryService _historyService;
        private readonly IPersonalAccountService _personalAccountService;
        private readonly IRankService _rankService;
        private readonly ISearchService _searchService;
        private readonly IApplicationUser _applicationUser;
        private readonly IMapper _mapper;

        public AdminController(
            IEventService eventService,
            IHistoryService historyService,
            ISearchService searchService,
            IRankService rankService,
            IPersonalAccountService personalAccountService,
            IApplicationUser applicationUser,
            IMapper mapper)
        {
            _eventService = eventService;
            _applicationUser = applicationUser;
            _searchService = searchService;
            _rankService = rankService;
            _personalAccountService = personalAccountService;
            _mapper = mapper;
            _historyService = historyService;
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

        [HttpPut("role/{login}")]
        public async Task<IActionResult> ChangeRole([Required] string role, string login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (role.ToLower() != Roles.Admin && role.ToLower() != Roles.User)
            {
                return BadRequest();
            }

            var result = await _personalAccountService.ChangeRoleAsync(UserId, login, role);

            if (result == ResultCode.NotFound) return NotFound();
            if (result == ResultCode.Forbidden) return Forbid();

            return StatusCode(result);
        }

        [HttpGet("history/{login}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<HistoryVm>> GetHistory(string login)
        {
            var result = await _historyService.GetHistoryAsync(login);

            if (result.Code == ResultCode.NotFound) return NotFound();

            return Ok(_mapper.Map<List<HistoryModel>, List<HistoryVm>>(result.Value));
        }

        [HttpPatch("balance/{login}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<HistoryVm>> ChangeBalance(string login, int change)
        {
            var resultCode = await _eventService.ChangeBalanceAsync(login, change);
            if (resultCode == ResultCode.NotFound) return NotFound();
            return Ok();
        }

        [HttpGet("users")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<HistoryVm>> GetPagedUsers([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 10 ? 10 : pageSize;

            var pagedModel = await _searchService.GetAllUsers(pageNumber, pageSize);

            var scores = pagedModel.Users
                .Select(x=> x.Score)
                .ToList();

            var ranks = await _rankService.GetRanks(scores);

            for (int i = 0; i < ranks.Count; i++)
            {
                pagedModel.Users[i].Rank = ranks[i];
            }

            return Ok(_mapper.Map<PagedUsersModel, PagedUserVm>(pagedModel));
        }
    }
}
