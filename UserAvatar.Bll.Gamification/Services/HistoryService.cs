using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryStorage _historyStorage;
        private readonly IUserStorage _userStorage;

        public HistoryService(IHistoryStorage historyStorage, IUserStorage userStorage)
        {
            _historyStorage = historyStorage;
            _userStorage = userStorage;
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
    }
}