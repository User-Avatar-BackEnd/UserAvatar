using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services
{
    public class EventService : IEventService
    {
        private readonly IEventStorage _eventStorage;
        private readonly IHistoryStorage _historyStorage;
        private readonly IUserStorage _userStorage;
        private readonly IMapper _mapper;

        public EventService(
            IEventStorage eventStorage,
            IHistoryStorage historyStorage,
            IMapper mapper,
            IUserStorage userStorage)
        {
            _eventStorage = eventStorage;
            _mapper = mapper;
            _historyStorage = historyStorage;
            _userStorage = userStorage;
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

        public async Task<int> ChangeBalance(string login, int balance)
        {
            var user = await _userStorage.GetByLoginAsync(login);
            if (user == null)
            {
                return ResultCode.NotFound;
            }

            await AddEventToHistoryAsync(user.Id, EventType.ChangeUserBalansByAdmin, balance);

            return ResultCode.Success;
        }

        public async Task AddEventToHistoryAsync(int userId, string eventType, int? customScore=null)
        {
            if (eventType == null) return;

            try
            {
                int score = await _eventStorage.GetScoreByNameAsync(eventType);
                if (customScore != null)
                {
                    score = (int)customScore;
                }

                var history = new History
                {
                    DateTime = DateTimeOffset.UtcNow,
                    Calculated = false,
                    UserId = userId,
                    EventName = eventType,
                    Score = score
                };
                await _historyStorage.AddHstoryAsync(history);
            }
            catch (Exception) { }

        }

        public async Task<Result<List<HistoryModel>>> GetHistoryAsync(string login)
        {
            var user = await _userStorage.GetByLoginAsync(login);
            if (user == null)
            {
                return new Result<List<HistoryModel>>(ResultCode.NotFound);
            }

            var history = await _historyStorage.GetHistoryByUserAsync(user.Id);

            var historyModels = _mapper.Map<List<History>, List<HistoryModel>>(history);

            return new Result<List<HistoryModel>>(historyModels);
        }
    }
}
