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

        Task AddEventToHistoryAsync(int userId, string eventType);

        Task<Result<List<HistoryModel>>> GetHistoryAsync(string login);
    }
}
