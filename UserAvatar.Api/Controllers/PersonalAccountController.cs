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
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Gamification.Models;

namespace UserAvatar.Api.Controllers
{
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
        
        public PersonalAccountController(
            IPersonalAccountService personalAccountService,
            IRateService rateService,
            IRankService rankService,
            IMapper mapper, 
            IInviteService inviteService, 
            IApplicationUser applicationUser)
        {
            _personalAccountService = personalAccountService;
            _rateService = rateService;
            _rankService = rankService;
            _mapper = mapper;
            _inviteService = inviteService;
            _applicationUser = applicationUser;
        }
        
        private int UserId => _applicationUser.Id;

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

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserDataVm>> GetUserDataAsync()
        {
            var userData = await _personalAccountService.GetUsersDataAsync(UserId);
            var rankData = await _rankService.GetAllRanksDataAsync(userData.Value.Score);

            var userDataVm = new UserDataVm
            {
                Email = userData.Value.Email,
                Login = userData.Value.Login,
                Role = userData.Value.Role,
                InvitesAmount = userData.Value.Invited
                    .Count(invite => invite.Status == -1),
                Rank = rankData.Name,
                PreviousLevelScore = rankData.Score,
                CurrentScoreAmount = userData.Value.Score,
                NextLevelScore = userData.Value.Score >= 1000 ? userData.Value.Score : rankData.MaxScores
            };

            return Ok(userDataVm);
        }

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

            if (resultCode == ResultCode.NotFound) return NotFound();
            if (resultCode == ResultCode.NotFound) return Forbid();

            return Ok();
        }

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