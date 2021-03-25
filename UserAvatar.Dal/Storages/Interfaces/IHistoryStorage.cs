using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IHistoryStorage
    {
        Task AddHistory(History history);

        Task<bool> GetNotCalculatedHistory();
        Task SaveChanges();

        Task<List<History>> GetUserScoresList();
    }
}
