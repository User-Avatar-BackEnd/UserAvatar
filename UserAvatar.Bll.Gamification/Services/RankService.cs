﻿using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services
{
    public class RankService : IRankService
    {
        private readonly IRankStorage _rankStorage;
        private readonly IMapper _mapper;

        public RankService(IRankStorage rankStorage,
            IMapper mapper)
        {
            _rankStorage = rankStorage;
            _mapper = mapper;
        }

        // todo : change
        public async Task<RankDataModel> GetAllRanksData(int score)
        {
            // вынести в конструктор? вызывется каждый раз
            var ranks = await _rankStorage.GetAllRankAsync();

            var fullRanks = _mapper.Map<List<Rank>,List<RankDataModel>>(ranks);

            fullRanks = SetMaxScore(fullRanks);

            var smt = fullRanks.First(x => score < x.MaxScores && score >= x.Score);

            return smt;
        }

        public async Task<List<string>> GetRanks(List<int> scores)
        {
            var ranks = await _rankStorage.GetAllRankAsync();

            var fullRanks = _mapper.Map<List<Rank>, List<RankDataModel>>(ranks);

            fullRanks = SetMaxScore(fullRanks);

            return  scores.Select(score => fullRanks
            .First(x => score < x.MaxScores && score >= x.Score).Name)
                .ToList();
        }

        private List<RankDataModel> SetMaxScore(List<RankDataModel> ranks)
        {
            for (int i = 0, j = 1; i < ranks.Count - 1; i++, j++)
            {
                ranks[i].MaxScores = ranks[j].Score;
            }

            ranks[ranks.Count - 1].MaxScores = int.MaxValue;

            return ranks;
        }
    }
}
