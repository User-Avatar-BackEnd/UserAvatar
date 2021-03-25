using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Options;
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
        
        public async Task<Result<bool>> CreateInviteAsync(
            int boardId, int userId, string payload)
        {
            if (payload == null)
                return new Result<bool>(ResultCode.NotFound);
            
            if (await _boardStorage.GetBoardAsync(boardId) == null)
                return new Result<bool>(ResultCode.NotFound);

            var invitedId = await GetUserIdByPayload(payload);
            
            if (invitedId == ResultCode.UserNotFound)
                return new Result<bool>(ResultCode.NotFound);
            
            if (userId == invitedId || !await _boardStorage.IsUserBoardAsync(userId,boardId))
                return new Result<bool>(ResultCode.Forbidden);

            if (await _boardStorage.IsUserBoardAsync(invitedId, boardId))
            {
                return new Result<bool>(ResultCode.LoginAlreadyExist);
            }
            var thisInvite = await _inviteStorage.GetInviteByBoardAsync(userId,invitedId, boardId);
                
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
                return new Result<bool>(true,EventType.SendInvite);
            }
            await _inviteStorage.CreateAsync(thisInvite);
            return new Result<bool>(true, EventType.SendInvite);

        }

        public async Task<Result<List<UserModel>>> FindByQuery(int boardId, int userId, string query)
        {
            if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
                return new Result<List<UserModel>>(ResultCode.Forbidden);
            
            var userList = await _userStorage.InviteByQuery(boardId, query);
            
            return userList.Count == 0 
                ? new Result<List<UserModel>>(ResultCode.NotFound) 
                : new Result<List<UserModel>>(_mapper.Map<List<User>, List<UserModel>>(userList));
        }

        public async Task<int> UpdateInviteAsync(int inviteId, int userId, int statusCode)
        {
            var thisInvite = await _inviteStorage.GetByIdAsync(inviteId);

            
                if (thisInvite == null 
                    || await _userStorage.GetByIdAsync(userId) == null
                    || thisInvite.Status == InviteStatus.Accepted
                    || await _boardStorage.IsUserBoardAsync(thisInvite.InvitedId, thisInvite.BoardId))
                return ResultCode.NotFound;
                
                if (thisInvite.InvitedId != userId)
                    return ResultCode.Forbidden;

            if (statusCode == InviteStatus.Accepted)
            {
                await _boardStorage.AddAsMemberAsync(new Member
                {
                    UserId = thisInvite.InvitedId,
                    BoardId = thisInvite.BoardId,
                });
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