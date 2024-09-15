using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services;

public sealed class HistoryService : IHistoryService
{
    private readonly IEventStorage _eventStorage;
    private readonly IHistoryStorage _historyStorage;
    private readonly ILogger<HistoryService> _logger;
    private readonly IMapper _mapper;
    private readonly IUserStorage _userStorage;

    public HistoryService(
        IHistoryStorage historyStorage,
        IUserStorage userStorage,
        IEventStorage eventStorage,
        IMapper mapper,
        ILogger<HistoryService> logger)
    {
        _historyStorage = historyStorage;
        _userStorage = userStorage;
        _eventStorage = eventStorage;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task MakeScoreTransactionAsync()
    {
        if (await _historyStorage.GetNotCalculatedHistoryAsync())
        {
            var getUserHistoryList = await _historyStorage.GetUserScoresListAsync();
            foreach (var history in getUserHistoryList)
            {
                var thisUser = await _userStorage.GetByIdAsync(history.UserId);
                thisUser.Score += history.Score;
                if (thisUser.Score < 0)
                {
                    thisUser.Score = 0;
                }
            }

            await _historyStorage.SaveChangesAsync();
        }
    }

    public async Task AddEventToHistoryAsync(int userId, string eventType, int? customScore = null)
    {
        if (eventType == null)
        {
            return;
        }

        try
        {
            var score = await _eventStorage.GetScoreByNameAsync(eventType);
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
            var userDailyQuest = await _eventStorage.GetUserDailyQuestById(userId);
            if (userDailyQuest != null
                && userDailyQuest.EventName == eventType
                && !userDailyQuest.IsCompleted)
            {
                userDailyQuest.IsCompleted = true;
                history.Score = score * 2;
            }

            await _historyStorage.AddHstoryAsync(history);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Crashed at AddEventToHistoryAsync. HistoryService");
        }
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
