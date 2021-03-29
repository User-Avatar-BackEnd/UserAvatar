using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using System.Net.Mime;
using System.Net;
using UserAvatar.Api.Authentication;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Gamification.Models;

namespace UserAvatar.Api.Controllers
{
    /// <summary>
    /// Personal account controller
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/account")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public class PersonalAccountController : ControllerBase
    {
        private readonly IPersonalAccountService _personalAccountService;
        private readonly IRateService _rateService;
        private readonly IRankService _rankService;
        private readonly IInviteService _inviteService;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _applicationUser;
        private readonly IEventService _eventService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="personalAccountService">personal account service</param>
        /// <param name="rateService">service for user rate</param>
        /// <param name="rankService">service with ranks</param>
        /// <param name="mapper">automapper</param>
        /// <param name="inviteService">service for invites</param>
        /// <param name="applicationUser">this user id</param>
        /// <param name="eventService">event service</param>
        public PersonalAccountController(
            IPersonalAccountService personalAccountService,
            IRateService rateService,
            IRankService rankService,
            IMapper mapper, 
            IInviteService inviteService, 
            IApplicationUser applicationUser,
            IEventService eventService)
        {
            _personalAccountService = personalAccountService;
            _rateService = rateService;
            _rankService = rankService;
            _mapper = mapper;
            _inviteService = inviteService;
            _applicationUser = applicationUser;
            _eventService = eventService;
        }
        
        private int UserId => _applicationUser.Id;

        /// <summary>
        /// Changes this user login
        /// </summary>
        /// <param name="login">new user login</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> ChangeLoginAsync(ChangeLoginRequest login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            login.Login = login.Login.Trim();

            var result = await _personalAccountService.ChangeLoginAsync(UserId, login.Login);

            if (result == ResultCode.NotFound) return NotFound();

            if (result != ResultCode.Success)
            {
                return Conflict(result);
            }

            return StatusCode(result);
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="request">password body</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("password")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _personalAccountService.ChangePasswordAsync(UserId, request.OldPassword, request.NewPassword);

            if (result == ResultCode.NotFound) return NotFound();

            if (result != ResultCode.Success)
            {
                return Conflict(result);
            }

            return StatusCode(result);
        }

        /// <summary>
        /// Gets user data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserDataVm>> GetUserDataAsync()
        {
            var userData = await _personalAccountService.GetUsersDataAsync(UserId);
            var rankData = await _rankService.GetAllRanksDataAsync(userData.Value.Score);

            var thisUserDailyEvent = _mapper.Map<DailyEventVm>(await _eventService.GetUserDailyEvent(UserId));

            var userDataVm = new UserDataVm
            {
                Email = userData.Value.Email,
                Login = userData.Value.Login,
                Role = userData.Value.Role,
                InvitesAmount = userData.Value.Invited
                    .Count(invite => invite.Status == 0),
                DailyEvent = thisUserDailyEvent,
                Rank = rankData.Name,
                PreviousLevelScore = rankData.Score,
                CurrentScoreAmount = userData.Value.Score,
                NextLevelScore = userData.Value.Score >= 1000 ? userData.Value.Score : rankData.MaxScores
            };

            return Ok(userDataVm);
        }

        /// <summary>
        /// Gets all user invites
        /// </summary>
        /// <returns></returns>
        [HttpGet("invites")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<InviteVm>>> GetAllInvitesAsync()
        {
            var result = await _inviteService.GetAllInvitesAsync(UserId);
            
            if (result.Code == ResultCode.NotFound) return NotFound();
            
            var scores = result.Value.Select(invite => invite.Inviter.Score).ToList();
            var ranks = await _rankService.GetRanksAsync(scores);

            var resultedOutput = _mapper.Map<List<InviteModel>, List<InviteVm>>(result.Value);

            for (var i = 0; i < resultedOutput.Count; i++)
                resultedOutput[i].Inviter.Rank = ranks[i];

            return Ok(resultedOutput);
        }

        /// <summary>
        /// Accepts or declines invites
        /// </summary>
        /// <param name="status">new invite status</param>
        /// <param name="inviteId">id of invite</param>
        /// <returns></returns>
        [HttpPatch("invites/{inviteId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateInvitationAsync([FromQuery]int status, int inviteId)
        {
            if (status != -1 && status !=1)
            {
                return BadRequest("Status may be only -1 or 1");
            }

            var resultCode = await _inviteService.UpdateInviteAsync(inviteId, UserId, status);

            return resultCode switch
            {
                ResultCode.NotFound => NotFound(),
                ResultCode.Forbidden => Forbid(),
                _ => Ok()
            };
        }

        /// <summary>
        /// Gets rates
        /// </summary>
        /// <returns></returns>
        [HttpGet("rate")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<FullRateVm>> GetRate()
        {
           var rate = await _rateService.GetTopRateAsync(UserId);

            return Ok(_mapper.Map<FullRateModel, FullRateVm>(rate.Value));
        }
    }
}
