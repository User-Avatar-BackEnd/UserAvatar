using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IHistoryStorage
    {
        Task AddHstoryAsync(History history);

        Task<List<History>> GetHistoryByUserAsync(int userId);

        Task<bool> GetNotCalculatedHistory();

        Task SaveChanges();

        Task<List<History>> GetUserScoresList();
    }
}
