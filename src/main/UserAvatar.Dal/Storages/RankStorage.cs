using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Dal.Storages;

public sealed class RankStorage : IRankStorage
{
    private readonly UserAvatarContext _dbContext;

    public RankStorage(UserAvatarContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Rank>> GetAllRankAsync()
    {
        return await _dbContext.Ranks.OrderBy(x => x.Score)
            .ToListAsync();
    }
}
