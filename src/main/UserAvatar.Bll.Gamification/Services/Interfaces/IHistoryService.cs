using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Infrastructure;

namespace UserAvatar.Bll.Gamification.Services.Interfaces;

public interface IHistoryService
{
    Task MakeScoreTransactionAsync();

    Task AddEventToHistoryAsync(int userId, string eventType, int? customScore = null);

    Task<Result<List<HistoryModel>>> GetHistoryAsync(string login);
}
