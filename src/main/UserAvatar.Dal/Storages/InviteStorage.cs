﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages;

public sealed class InviteStorage : IInviteStorage
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

    public async Task<Invite> GetInviteByBoardAsync(int userId, int invatedId, int boardId)
    {
        //todo: implement!
        return await Task.FromResult(_userAvatarContext.Invites
            .FirstOrDefault(x => x.InvitedId == invatedId && x.BoardId == boardId && x.InviterId == userId));
    }

    public async Task<List<Invite>> GetInvitesAsync(int userId)
    {
        //???????????????????????????????????????????????????
        return await Task.FromResult(_userAvatarContext.Invites
            .Include(x => x.Board)
            .Include(x => x.Inviter)
            .Where(x => x.InvitedId == userId && x.Status == 0).ToList());
    }
}
