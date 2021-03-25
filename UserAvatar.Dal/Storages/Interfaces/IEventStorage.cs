using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IEventStorage
    {
        Task<List<Event>> GetEventListAsync();

        Task UpdateEventsAsync(List<Event> events);

        Task<int> GetScoreByNameAsync(string name);
    }
}
