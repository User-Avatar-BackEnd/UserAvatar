using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;

namespace UserAvatar.Bll.Gamification.Services.Interfaces
{
    public interface IRankService
    {
        Task<RankDataModel> GetRank(int score);
    }
}
