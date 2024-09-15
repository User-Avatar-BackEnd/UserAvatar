using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services;

public sealed class RankService : IRankService
{
    private readonly IMapper _mapper;
    private readonly IRankStorage _rankStorage;

    public RankService(
        IRankStorage rankStorage,
        IMapper mapper)
    {
        _rankStorage = rankStorage;
        _mapper = mapper;
    }

    public async Task<RankDataModel> GetAllRanksDataAsync(int score)
    {
        var ranks = await _rankStorage.GetAllRankAsync();

        var fullRanks = SetMaxScore(_mapper.Map<List<Rank>, List<RankDataModel>>(ranks));

        return fullRanks.First(x => score < x.MaxScores && score >= x.Score);
    }

    public async Task<List<string>> GetRanksAsync(List<int> scores)
    {
        var ranks = await _rankStorage.GetAllRankAsync();

        var fullRanks = SetMaxScore(_mapper.Map<List<Rank>, List<RankDataModel>>(ranks));

        return scores
            .Select(score => fullRanks
                .First(x => score < x.MaxScores && score >= x.Score).Name)
            .ToList();
    }

    private List<RankDataModel> SetMaxScore(List<RankDataModel> ranks)
    {
        for (var i = 0; i < ranks.Count - 1; i++)
        {
            ranks[i].MaxScores = ranks[i + 1].Score;
        }

        ranks[^1].MaxScores = int.MaxValue;

        return ranks;
    }
}
