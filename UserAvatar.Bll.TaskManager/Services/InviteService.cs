using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.TaskManager.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
{
    public class InviteService : IInviteService
    {
        private readonly IInviteStorage _inviteStorage;
        private readonly IUserStorage _userStorage;
        private readonly IBoardStorage _boardStorage;
        private readonly IMapper _mapper;

        public InviteService(
            IInviteStorage inviteStorage, 
            IMapper mapper, 
            IUserStorage userStorage, 
            IBoardStorage boardStorage)
        {
            _inviteStorage = inviteStorage;
            _mapper = mapper;
            _userStorage = userStorage;
            _boardStorage = boardStorage;
        }

        private async Task<int> GetUserIdByPayload(string payload)
        {
            if (int.TryParse(payload, out var invitedId) 
                && await _userStorage.GetByIdAsync(invitedId) != null)
                return invitedId;
            
            var thisUser = await _userStorage.GetByEmailAsync(payload);
            return thisUser?.Id ?? ResultCode.UserNotFound;
        }
        private async Task<Invite> GetInvite(int boardId, int userId)
        {
            return await _inviteStorage.GetInviteByBoardAsync(userId, boardId);
        }
        public async Task<int> CreateInviteAsync(
            int boardId, int userId, string payload)
        {
            if (payload == null)
                return ResultCode.NotFound;
            var invitedId = await GetUserIdByPayload(payload);
            
            if (invitedId == ResultCode.UserNotFound)
                return ResultCode.NotFound;
            
            if (userId == invitedId)
                return ResultCode.Forbidden;
            if (await _boardStorage.GetBoardAsync(boardId) == null)
                return ResultCode.NotFound;

            var thisInvite = await _inviteStorage.GetInviteByBoardAsync(invitedId, boardId);
            
            if(!await _boardStorage.IsUserBoardAsync(invitedId, boardId))
                if (thisInvite == null)
                {
                    thisInvite = new Invite
                    {
                        InviterId = userId,
                        BoardId = boardId,
                        InvitedId = invitedId,
                        Issued = DateTimeOffset.UtcNow,
                        Status = InviteStatus.Pending
                    };
                }
                else
                {
                    thisInvite.Status = InviteStatus.Pending;
                    thisInvite.Issued = DateTimeOffset.UtcNow;
                    await _inviteStorage.UpdateAsync(thisInvite);
                    return ResultCode.Success;
                }

            await _inviteStorage.CreateAsync(thisInvite);
            return ResultCode.Success;
        }

        public async Task<Result<List<UserModel>>> FindByQuery(string query)
        {
            //todo:
            var userList = await _userStorage.FindByQuery(query);

            return new Result<List<UserModel>>(_mapper.Map<List<User>, List<UserModel>>(userList));

        }
        
        public async Task<int> UpdateInviteAsync(int inviteId, int userId, int statusCode)
        {
            var thisInvite = await _inviteStorage.GetByIdAsync(inviteId);
            if (thisInvite == null || await _userStorage.GetByIdAsync(userId) == null)
                return ResultCode.NotFound;

            if (statusCode == InviteStatus.Accepted)
            {
                await _boardStorage.AddAsMemberAsync(thisInvite.InvitedId, thisInvite.BoardId);
            }
           
            thisInvite.Status = statusCode;
            
            await _inviteStorage.UpdateAsync(thisInvite);
            return ResultCode.Success;
        }

        public async Task<Result<List<InviteModel>>> GetAllInvitesAsync(int userId)
        {
            if(await _userStorage.GetByIdAsync(userId) == null)
                return new Result<List<InviteModel>>(ResultCode.NotFound);
            var inviteList = await _inviteStorage.GetInvitesAsync(userId);
            return new Result<List<InviteModel>>(_mapper.Map<List<Invite>,List<InviteModel>>(inviteList));
        }
    }
}