using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IHistoryStorage
    {
        Task AddHstoryAsync(History history);

        Task<List<History>> GetHistoryByUserAsync(int userId);
    }
}
