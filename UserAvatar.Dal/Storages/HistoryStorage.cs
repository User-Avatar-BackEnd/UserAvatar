using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Z.EntityFramework.Plus;

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
                .OrderByDescending(x => x.DateTime)
                .Take(100)
                .ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _userAvatarContext.Histories.Where(x => !x.Calculated)
                .UpdateAsync(x => new History{Calculated = true});
            await _userAvatarContext.SaveChangesAsync();
        }

        public async Task<User> GetThisUser(int userId)
        {
            return await _userAvatarContext.Users.FindAsync(userId);
        }

        public async Task<bool> GetNotCalculatedHistory()
        {
            return await _userAvatarContext.Histories.AnyAsync(x => !x.Calculated);
        }

        public async Task<List<History>> GetUserScoresList()
        {
            var query = await _userAvatarContext.Histories
                .Where(x=> !x.Calculated)
                .GroupBy(x => x.UserId)
                .Select(g => new History
                {
                    UserId = g.Key,
                    Score = g.Sum(x => x.Score)
                }).ToListAsync();

            return query;
        }

        public async Task<List<History>> GetHistoryById(int userId)
        {
            return await _userAvatarContext.Histories
                .Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
