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
    }
}
