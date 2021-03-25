using System.Threading.Tasks;

namespace UserAvatar.Bll.Gamification.Services.Interfaces
{
    public interface IHistoryService
    {
        Task MakeScoreTransaction();
    }
}