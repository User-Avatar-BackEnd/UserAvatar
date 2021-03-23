using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using UserAvatar.Api.Contracts.Dtos;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonalAccountController : ControllerBase
    {
        private readonly IPersonalAccountService _personalAccountService;
        // unnesessary di
        private readonly IMapper _mapper;
        

        public PersonalAccountController(IPersonalAccountService personalAccountService,IMapper mapper)
        {
            _personalAccountService = personalAccountService;
            _mapper = mapper;
        }

        [HttpPatch]
        [Route("change_login")]
        public async Task<ActionResult> ChangeLoginAsync([FromBody] string login)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            login = login.Trim();

            if (login.Length < 5 || login.Length > 64) throw new SystemException("Login length should be between 5 and 64 characters");

            await _personalAccountService.ChangeLoginAsync(userId, login);

            return Ok();
        }

        [HttpPatch]
        [Route("change_password")]
        public async Task<ActionResult> ChangePaswordAsync(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            await _personalAccountService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<UserDataDto>> GetUserDataAsync()
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            var userData = await _personalAccountService.GetUsersDataAsync(userId);
            // ToDo: get the rest of the needed data from GamificationService

            var userDataDto = new UserDataDto()
            {
                Email = userData.Email,
                Login = userData.Login,
                InvitesAmount = userData.Invited
                              .Where(invite => invite.Status == -1)
                              .Count(),

                // ToDo: set the rest of the properties =>
                Rank = "Cossack",
                PreviousLevelScore = 100,
                CurrentScoreAmount = 175,
                NextLevelScore = 300
            };

            return Ok(userDataDto);
        }
    }
}