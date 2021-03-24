using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface IRankStorage
    {
        Task<string> GetUsersRank(int userId);
    }
}
