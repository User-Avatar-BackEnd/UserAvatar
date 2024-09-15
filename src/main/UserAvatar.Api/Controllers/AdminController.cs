using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Authentication;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers;

/// <summary>
///     Administrator controller
/// </summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = Roles.Admin)]
[ProducesResponseType((int)HttpStatusCode.Forbidden)]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public sealed class AdminController : ControllerBase
{
    private readonly IApplicationUser _applicationUser;
    private readonly IEventService _eventService;
    private readonly IHistoryService _historyService;
    private readonly IMapper _mapper;
    private readonly IPersonalAccountService _personalAccountService;
    private readonly IRankService _rankService;
    private readonly ISearchService _searchService;

    /// <summary>
    ///     Constructio
    /// </summary>
    /// <param name="eventService">event service</param>
    /// <param name="historyService">history service</param>
    /// <param name="searchService">search service</param>
    /// <param name="rankService">rank service</param>
    /// <param name="personalAccountService">personal account service</param>
    /// <param name="applicationUser">this user id</param>
    /// <param name="mapper">mapper</param>
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

    /// <summary>
    ///     Gets all events
    /// </summary>
    /// <returns></returns>
    [HttpGet("events")]
    [ProducesResponseType(typeof(List<EventVm>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<EventVm>>> GetEventsAsync()
    {
        var events = await _eventService.GetEventListAsync();
        return Ok(_mapper.Map<List<EventModel>, List<EventVm>>(events));
    }

    /// <summary>
    ///     Changing cost of event
    /// </summary>
    /// <param name="newEvents"></param>
    /// <returns></returns>
    [HttpPut("events")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangeEventsCostAsync(List<EventDto> newEvents)
    {
        var eventModels = _mapper.Map<List<EventDto>, List<EventModel>>(newEvents);

        var result = await _eventService.ChangeEventsCostAsync(eventModels);

        if (result == ResultCode.BadRequest)
        {
            return BadRequest();
        }

        return Ok();
    }

    /// <summary>
    ///     Changing user role
    /// </summary>
    /// <param name="role"></param>
    /// <param name="login"></param>
    /// <returns></returns>
    [HttpPut("role/{login}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> ChangeRoleAsync([Required] string role, string login)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (role.ToLower() != Roles.Admin && role.ToLower() != Roles.User)
        {
            return BadRequest();
        }

        var result = await _personalAccountService.ChangeRoleAsync(UserId, login, role);

        if (result == ResultCode.NotFound)
        {
            return NotFound();
        }

        if (result == ResultCode.Forbidden)
        {
            return Forbid();
        }

        return StatusCode(result);
    }

    /// <summary>
    ///     Gets history
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [HttpGet("history/{login}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<HistoryVm>> GetHistory(string login)
    {
        var result = await _historyService.GetHistoryAsync(login);

        if (result.Code == ResultCode.NotFound)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<List<HistoryModel>, List<HistoryVm>>(result.Value));
    }

    /// <summary>
    ///     Changing user balance
    /// </summary>
    /// <param name="login"></param>
    /// <param name="change"></param>
    /// <returns></returns>
    [HttpPatch("balance/{login}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<HistoryVm>> ChangeBalanceAsync(string login, int change)
    {
        var resultCode = await _eventService.ChangeBalanceAsync(login, change);

        if (resultCode == ResultCode.NotFound)
        {
            return NotFound();
        }

        return Ok();
    }

    /// <summary>
    ///     Getting all users paged
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("users")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<HistoryVm>> GetPagedUsers([FromQuery] int pageNumber, [FromQuery] int pageSize,
        [FromQuery] string query)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 10 ? 10 : pageSize;

        query = query == null ? "" : query.Trim();

        var pagedModel = await _searchService.GetAllUsersAsync(pageNumber, pageSize, query);

        var scores = pagedModel.Users.Select(x => x.Score).ToList();

        var ranks = await _rankService.GetRanksAsync(scores);

        for (var i = 0; i < ranks.Count; i++)
        {
            pagedModel.Users[i].Rank = ranks[i];
        }

        return Ok(_mapper.Map<PagedUsersModel, PagedUserVm>(pagedModel));
    }
}
