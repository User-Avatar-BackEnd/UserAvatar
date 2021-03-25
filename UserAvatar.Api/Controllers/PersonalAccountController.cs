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
        private readonly IHistoryService _historyService;
        

        public PersonalAccountController(IPersonalAccountService personalAccountService,
            IRateService rateService,
            IRankService rankService,
            IMapper mapper, 
            IInviteService inviteService, 
            IApplicationUser applicationUser, IHistoryService historyService)
        {
            _personalAccountService = personalAccountService;
            _rateService = rateService;
            _rankService = rankService;
            _mapper = mapper;
            _inviteService = inviteService;
            _applicationUser = applicationUser;
            _historyService = historyService;
        }
        
        private int UserId => _applicationUser.Id;

        [HttpPatch]
        [Route("login")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> ChangeLoginAsync(ChangeLoginRequest login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            login.Login = login.Login.Trim();

            await _personalAccountService.ChangeLoginAsync(UserId, login.Login);

            return Ok();
        }

        [HttpPatch]
        [Route("password")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _personalAccountService.ChangePasswordAsync(UserId, request.OldPassword, request.NewPassword);

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<UserDataVm>> GetUserDataAsync()
        {
            var userData = await _personalAccountService.GetUsersDataAsync(UserId);
            var rankData = await _rankService.GetRank(userData.Score);

            var userDataVm = new UserDataVm
            {
                Email = userData.Email,
                Login = userData.Login,
                Role = userData.Role,
                InvitesAmount = userData.Invited
                    .Count(invite => invite.Status == -1),
                Rank = rankData.Name,
                PreviousLevelScore = rankData.Score,
                CurrentScoreAmount = userData.Score,
                NextLevelScore = userData.Score >= 1000 ? userData.Score : rankData.MaxScores
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

            return Ok(_mapper.Map<List<InviteModel>,List<InviteVm>>(result.Value));
        }

        [HttpPatch("invites/{inviteId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateInvitationAsync([FromQuery]int status, int inviteId)
        {
            if (Math.Abs(status) > 1) return BadRequest("Status may be only -1 0 1");
            var resultCode = await _inviteService.UpdateInviteAsync(inviteId, UserId, status);

            if (resultCode == ResultCode.NotFound) return NotFound();

            return Ok();
        }

        [HttpGet("rate")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<FullRateVm>> GetRate()
        {
           var rate = await _rateService.GetTopRate(UserId);

            return Ok(_mapper.Map<FullRateModel, FullRateVm>(rate.Value));
        }
    }
}