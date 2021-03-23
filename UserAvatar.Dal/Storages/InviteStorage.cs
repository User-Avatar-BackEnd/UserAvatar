using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages
{
    public class InviteStorage : IInviteStorage
    {
        private readonly UserAvatarContext _userAvatarContext;

        public InviteStorage(UserAvatarContext userAvatarContext)
        {
            _userAvatarContext = userAvatarContext;
        }

        public async Task CreateAsync(Invite invite)
        {
            await _userAvatarContext.AddAsync(invite);
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invite invite)
        {
            _userAvatarContext.Update(invite);
            await _userAvatarContext.SaveChangesAsync();
        }
        
        public async Task<Invite> GetByIdAsync(int inviteId)
        {
            return await _userAvatarContext.Invites.FindAsync(inviteId);
        } 
        
        public async Task<Invite> GetInviteByBoardAsync(int userId, int boardId)
        { 
            //todo: implement!
            return await Task.FromResult(_userAvatarContext.Invites.FirstOrDefault(x => x.InvitedId == userId && x.BoardId == boardId));
        }
        
        public async Task<List<Invite>> GetInvitesAsync(int userId)
        {
            //???????????????????????????????????????????????????
            return await Task.FromResult(_userAvatarContext.Invites.Where(x => x.InvitedId == userId && x.Status == 0).ToList());
        }
        
        public bool IsUserInviteD(int inviteId, int userId)
        {
            return _userAvatarContext.Invites.Any(x => x.Id == inviteId && x.InvitedId == userId);
        }
        public bool IsUserInviteR(int inviteId, int userId)
        {
            return _userAvatarContext.Invites.Any(x => x.Id == inviteId && x.InviterId == userId);
        }
    }
}