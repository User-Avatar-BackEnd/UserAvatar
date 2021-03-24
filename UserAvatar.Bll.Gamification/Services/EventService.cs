using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
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

        public async Task<int> ChangeEventsCostAsync(List<EventModel> newEvents)
        {
            var events = await _eventStorage.GetEventListAsync();
            events = events.Where(x => x.Score != -1).ToList();

            if (newEvents.Count != events.Count ||
                !newEvents.All(x => events.Any(y => y.Name == x.Name)))
            {
                return ResultCode.BadRequest;
            }

            events.ForEach(x => x.Score = newEvents.First(y => y.Name == x.Name).Score);

            await _eventStorage.UpdateEventsAsync(events);

            return ResultCode.Success;
        }
    }
}
