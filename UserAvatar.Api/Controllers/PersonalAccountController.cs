using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonalAccountController : ControllerBase
    {
        private readonly IPersonalAccountService _personalAccountService;
        private readonly IMapper _mapper;

        public PersonalAccountController(IPersonalAccountService personalAccountService,IMapper mapper)
        {
            _personalAccountService = personalAccountService;
            _mapper = mapper;
        }

        [HttpPatch]
        [Route("change_login")]
        public async Task<ActionResult> ChangeLogin([FromBody] string login)
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            login = login.Trim();

            if (login.Length < 5 || login.Length > 64) throw new SystemException("Login length should be between 5 and 64 characters");

            await _personalAccountService.ChangeLoginAsync(userId, login);

            return Ok();
        }

        [HttpPatch]
        [Route("change_password")]
        public async Task<ActionResult> ChangePasword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Convert.ToInt32(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

            await _personalAccountService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);

            return Ok();
        }
    }
}