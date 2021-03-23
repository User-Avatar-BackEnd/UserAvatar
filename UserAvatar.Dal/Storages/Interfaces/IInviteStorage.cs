﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IInviteStorage
    {
        Task CreateAsync(Invite invite);
        Task UpdateAsync(Invite invite);
        Task<Invite> GetByIdAsync(int inviteId);
        Task<List<Invite>> GetInvitesAsync(int userId);
        Task<Invite> GetInviteByBoardAsync(int userId, int boardId);
    }
}