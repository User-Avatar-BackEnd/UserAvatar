using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;

namespace UserAvatar.Bll.Gamification.Services.Interfaces
{
    public interface IRankService
    {
        Task<RankDataModel> GetAllRanksData(int score);
        Task<List<string>> GetRanks(List<int> scores);
    }
}
