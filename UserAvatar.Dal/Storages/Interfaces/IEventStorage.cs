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
        Task BulkInsertDailyQuestsAsync(List<DailyEvent> quests);
        //Killer features
        Task<DailyEvent> GetUserDailyQuestById(int userid);
        Task<int> DeleteAllDailyEventsAsync();
    }
}
