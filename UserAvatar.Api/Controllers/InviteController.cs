using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/invite")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class InviteController: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IInviteService _inviteService;
        private readonly IApplicationUser _applicationUser;
        
        public InviteController(IInviteService inviteService, IMapper mapper, IApplicationUser applicationUser)
        {
            _inviteService = inviteService;
            _mapper = mapper;
            _applicationUser = applicationUser;
        }
        
        private int UserId => _applicationUser.Id;
        
        [HttpPost]
        public async Task<IActionResult> CreateInvitationAsync(InviteDto inviteDto)
        {
            var resultCode = await _inviteService.CreateInviteAsync(inviteDto.BoardId, UserId, inviteDto.Payload);
            if (resultCode != ResultCode.Success)
            {
                return Conflict(resultCode);
            }

            return Ok();
        }

        [HttpGet("/findLogin")]
        public async Task<ActionResult<List<UserShortVm>>> GetUsersByQuery(string query)
        {
            throw new NotImplementedException();
        }
        
        [HttpPatch]
        public async Task<IActionResult> UpdateInvitationAsync(UpdateInviteDto updateInviteDto)
        {
            var resultCode = await _inviteService.UpdateInviteAsync(updateInviteDto.Id, UserId, updateInviteDto.Status);
            
            if (resultCode != ResultCode.Success)
            {
                return Conflict(resultCode);
            }

            return Ok();
        }
        
    }
}