using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages
{
    public class HistoryStorage : IHistoryStorage
    {
        private readonly UserAvatarContext _userAvatarContext;

        public HistoryStorage(UserAvatarContext userAvatarContext)
        {
            _userAvatarContext = userAvatarContext;
        }

        public async Task AddHstoryAsync(History history)
        {
            await _userAvatarContext.AddAsync(history);
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task<List<History>> GetHistoryByUserAsync(int userId)
        {
            return await _userAvatarContext.Histories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x=>x.DateTime)
                .Take(100)
                .ToListAsync();
        }
    }
}
