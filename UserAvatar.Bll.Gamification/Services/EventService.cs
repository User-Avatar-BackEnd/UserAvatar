using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services
{
    public class EventService : IEventService
    {
        private readonly IEventStorage _eventStorage;
        private readonly IMapper _mapper;

        public EventService(IEventStorage eventStorage, IMapper mapper)
        {
            _eventStorage = eventStorage;
            _mapper = mapper;
        }

        public async Task<List<EventModel>> GetEventListAsync()
        {
            var events = await _eventStorage.GetEventListAsync();

            var eventModels = _mapper.Map<IEnumerable<Event>, IEnumerable<EventModel>>(events);

            return eventModels.Where(x => x.Score != -1).ToList();
        }
    }
}
