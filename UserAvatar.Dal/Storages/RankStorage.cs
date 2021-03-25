using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Dal.Context;

namespace UserAvatar.Dal.Storages
{
    class RankStorage
    {
        private readonly UserAvatarContext _dbContext;

        public RankStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetAllRank(int scores)
        {
            //return await _dbContext.Ranks.OrderBy()
            throw new NotImplementedException();
        }
    }
}
