using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IRankStorage
    {
        Task<List<Rank>> GetAllRankAsync();
    }
}
