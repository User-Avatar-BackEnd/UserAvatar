using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Infrastructure;

namespace UserAvatar.Bll.Gamification.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventModel>> GetEventListAsync();

        Task<int> ChangeEventsCostAsync(List<EventModel> newEvents);

        Task<int> ChangeBalanceAsync(string login, int balance);
        
        //Killer feature
        Task GenerateDailyQuests();
        Task<DailyEventModel> GetUserDailyEvent(int userId);


    }
}
