using System;
using System.Threading.Tasks;
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

        public async Task AddHstory(History history)
        {
            await _userAvatarContext.AddAsync(history);
            await _userAvatarContext.SaveChangesAsync();
        }
    }
}
