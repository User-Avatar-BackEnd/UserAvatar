using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services
{
    public class RateService : IRateService
    {
        private readonly IUserStorage _userStorage;
        private readonly IRankService _rankService;
        private readonly IMapper _mapper;

        public RateService(IUserStorage userStorage, IRankService rankService, IMapper mapper)
        {
            _userStorage = userStorage;
            _rankService = rankService;
            _mapper = mapper;
        }

        public async Task<Result<FullRateModel>> GetTopRate(int userId)
        {
            var users = await _userStorage.GetUsersRate();

            var firstTen = users.Take(10).ToList();
            var underTopUsers = new List<RateModel>();

            #region filling the collections

            if (!firstTen.Any(x => x.Id == userId))
            {
                var currentUser = users.First(x => x.Id == userId);
                var currentUserIndex = users.IndexOf(currentUser);

                if (currentUserIndex == 11)
                {
                    underTopUsers.Add(_mapper.Map<User, RateModel>(currentUser));
                    underTopUsers.Add(_mapper.Map<User, RateModel>(users[currentUserIndex + 1]));
                }
                else
                if(currentUserIndex==(users.Count()-1))
                {
                    underTopUsers.Add(_mapper.Map<User, RateModel>(users[currentUserIndex - 1]));
                    underTopUsers.Add(_mapper.Map<User, RateModel>(currentUser));
                }
                else
                {
                    underTopUsers.Add(_mapper.Map<User,RateModel>(users[currentUserIndex - 1]));
                    underTopUsers.Add(_mapper.Map<User, RateModel>(currentUser));
                    underTopUsers.Add(_mapper.Map<User, RateModel>(users[currentUserIndex + 1]));
                }
            }

            // mapper.Map<List<?>,List<RateModel>>(first ten)
            var topUsers = _mapper.Map<List<RateModel>>(firstTen);

            #endregion

            //setting two properties for every in those collections : 

            #region FOR UNDERTOP COLLECTION

            if (underTopUsers.Count >= 2)
            {
                #region property isCurrentPlayer

                if (underTopUsers.Count == 2)
                {
                    var currentUser0 = underTopUsers.First(x => x.Id == userId);
                    var currentUserIndex0 = underTopUsers.IndexOf(currentUser0);

                    currentUser0.IsCurrentPlayer = true;

                    underTopUsers[currentUserIndex0] = currentUser0;
                    // зависит от ситуции: может быть первый, а может второй
                    //underTopUsers[0].IsCurrentPlayer = true;
                }
                else
                    underTopUsers[1].IsCurrentPlayer = true;
                #endregion

                #region property Rank
                foreach (var user in underTopUsers)
                {
                    var rank = await _rankService.GetAllRanksData(user.Score);
                    user.Rank = rank.Name;
                }

                //topUsers.ForEach(async x=> x.Rank = await _rankService.GetRank(x.Scores).R)
                #endregion

                #region property RatePosition
                foreach (var user in underTopUsers)
                {
                    user.RatePosition = users
                        .IndexOf(users
                            .First(x => x.Id == user.Id));
                }
                #endregion
            }

            #endregion

            #region FOR MAIN COLLECTION

            #region property isCurrentPlayer
            if (topUsers.Any(x => x.Id == userId))
            {
                var currentUser1 = topUsers.First(x => x.Id == userId);
                var currentUserIndex1 = topUsers.IndexOf(currentUser1);

                currentUser1.IsCurrentPlayer = true;

                topUsers[currentUserIndex1] = currentUser1;
            }
            #endregion

            #region property Rank
            foreach (var user in topUsers)
            {
                var rank = await _rankService.GetAllRanksData(user.Score);
                user.Rank = rank.Name;
            }
            #endregion

            #region property RatePosition
            foreach (var user in topUsers)
            {
                user.RatePosition = users
                    .IndexOf(users
                        .First(x => x.Id == user.Id));
            }
            #endregion

            #endregion

            var rate = new FullRateModel()
            {
                TopUsers = topUsers,
                UnderTopUsers = underTopUsers
            };

            return new Result<FullRateModel>(rate);
        }
    }
}
