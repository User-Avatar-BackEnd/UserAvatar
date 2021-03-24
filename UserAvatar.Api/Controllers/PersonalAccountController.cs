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
using UserAvatar.Bll.TaskManager.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonalAccountController : ControllerBase
    {
        private readonly IPersonalAccountService _personalAccountService;

        private readonly IInviteService _inviteService;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _applicationUser;
        

        public PersonalAccountController(IPersonalAccountService personalAccountService,
            IMapper mapper, 
            IInviteService inviteService, 
            IApplicationUser applicationUser)
        {
            _personalAccountService = personalAccountService;
            _mapper = mapper;
            _inviteService = inviteService;
            _applicationUser = applicationUser;
        }
        
        private int UserId => _applicationUser.Id;

        [HttpPatch]
        [Route("change_login")]
        public async Task<ActionResult> ChangeLoginAsync([FromBody] ChangeLoginRequest login)
        {
            login.Login = login.Login.Trim();

            if (login.Login.Length < 5 || login.Login.Length > 64) throw new SystemException("Login length should be between 5 and 64 characters");

            await _personalAccountService.ChangeLoginAsync(UserId, login.Login);

            return Ok();
        }

        [HttpPatch]
        [Route("change_password")]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _personalAccountService.ChangePasswordAsync(UserId, request.OldPassword, request.NewPassword);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<UserDataVm>> GetUserDataAsync()
        {
            var userData = await _personalAccountService.GetUsersDataAsync(UserId);
            // ToDo: get the rest of the needed data from GamificationService

            var userDataVm = new UserDataVm
            {
                //check if works
                Email = userData.Email,
                Login = userData.Login,
                InvitesAmount = userData.Invited
                    .Count(invite => invite.Status == -1),
                
                //Here needs to me
                
                /*InvitesAmount = userData.Invited
                    .Count(invite => invite.Status == -1),*/

                // ToDo: set the rest of the properties =>
                Rank = "Cossack",
                PreviousLevelScore = 100,
                CurrentScoreAmount = 175,
                NextLevelScore = 300
            };

            return Ok(userDataVm);
        }

        [HttpGet("/invites")]
        public async Task<ActionResult<List<InviteVm>>> GetAllInvitesAsync()
        {
            var result = await _inviteService.GetAllInvitesAsync(UserId);
            if (result.Code != ResultCode.Success)
                return NotFound(result);
            return Ok(_mapper.Map<List<InviteModel>,List<InviteVm>>(result.Value));
        }
    }
}