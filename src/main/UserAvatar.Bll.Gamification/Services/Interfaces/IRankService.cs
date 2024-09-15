using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;

namespace UserAvatar.Bll.Gamification.Services.Interfaces;

public interface IRankService
{
    Task<RankDataModel> GetAllRanksDataAsync(int score);

    Task<List<string>> GetRanksAsync(List<int> scores);
}
