using System.Collections.Generic;
using UserAvatar.Bll.Gamification.Models;

namespace UserAvatar.Bll.Gamification.Services.Interfaces
{
    public interface IEventService
    {
        public List<EventModel> GetEventList();
    }
}
