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
    public class EventStorage : IEventStorage
    {
        private readonly UserAvatarContext _dbContext;

        public EventStorage(UserAvatarContext userAvatarContext)
        {
            _dbContext = userAvatarContext;
        }

        public async Task<List<Event>> GetEventListAsync()
        {
            return await _dbContext.Events.ToListAsync();
        }

        public async Task UpdateEventsAsync(List<Event> events)
        {
            foreach(var ev in events)
            {
                _dbContext.Entry(ev).State = EntityState.Modified;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetScoreByName(string name)
        {
            return (await _dbContext.Events.FirstAsync(x => x.Name == name)).Score;
        }
    }
}
