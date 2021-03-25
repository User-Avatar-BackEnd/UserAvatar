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
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryStorage _historyStorage;
        private readonly IEventStorage _eventStorage;
        private readonly IUserStorage _userStorage;
        private readonly IMapper _mapper;

        public HistoryService(
            IHistoryStorage historyStorage,
            IUserStorage userStorage,
            IEventStorage eventStorage,
            IMapper mapper)
        {
            _historyStorage = historyStorage;
            _userStorage = userStorage;
            _eventStorage = eventStorage;
            _mapper = mapper;
        }

        public async Task MakeScoreTransaction()
        {
            if (await _historyStorage.GetNotCalculatedHistory())
            {
                var getUserHistoryList = await _historyStorage.GetUserScoresList();
                foreach (var history in getUserHistoryList)
                {
                    var thisUser = await _userStorage.GetByIdAsync(history.UserId);
                    thisUser.Score += history.Score;
                }
                await  _historyStorage.SaveChanges();
            }
        }

        public async Task AddEventToHistoryAsync(int userId, string eventType, int? customScore = null)
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