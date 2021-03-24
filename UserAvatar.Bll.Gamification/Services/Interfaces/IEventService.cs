using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;

namespace UserAvatar.Bll.Gamification.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventModel>> GetEventListAsync();

        Task<int> ChangeEventsCostAsync(List<EventModel> newEvents);
    }
}
