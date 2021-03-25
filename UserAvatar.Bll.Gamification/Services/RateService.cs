using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services
{
    public class RateService : IRateService
    {
        private readonly IUserStorage _userStorage;
        private readonly IMapper _mapper;

        public RateService(IUserStorage userStorage, IMapper mapper)
        {
            _userStorage = userStorage;
            _mapper = mapper;
        }

        public async Task<Result<List<RateModel>>> GetTopRate(int userId)
        {
            // todo: try to make 1 query in dal that will ret <- теперь это не нужно потому что у нас есть разбивка и логика этой разбивки?

            // Getting all existing rate sorted by descending
            var users = await _userStorage.GetUsersRate();

            var firstTen = users.Take(10).ToList();
            var underTopUsers = new List<RateModel>();

            #region filling the collections

            if (!firstTen.Any(x => x.Id == userId) && firstTen.Count > 10)
            {
                var currentUser = users.First(x => x.Id == userId);
                var currentUserIndex = users.IndexOf(currentUser);

                if (currentUserIndex == 11)
                {
                    underTopUsers.Add(_mapper.Map<RateModel>(currentUser));
                    underTopUsers.Add(_mapper.Map<RateModel>(users[currentUserIndex + 1]));
                }
                else
                {
                    underTopUsers.Add(_mapper.Map<RateModel>(users[currentUserIndex - 1]));
                    underTopUsers.Add(_mapper.Map<RateModel>(currentUser));
                    underTopUsers.Add(_mapper.Map<RateModel>(users[currentUserIndex + 1]));
                }
            }

            var topUsers = _mapper.Map<List<RateModel>>(firstTen);

            #endregion

            //setting two properties for every in those collections : 

            #region FOR UNDERTOP COLLECTION

            if (underTopUsers.Count >= 2)
            {
                #region property isCurrentPlayer

                if (underTopUsers.Count == 2)
                    underTopUsers[0].IsCurrentPlayer = true;
                else
                    underTopUsers[1].IsCurrentPlayer = true;
                #endregion

                #region property Rank
                foreach (var user in topUsers)
                {
                    // для каждого вызвать метод сервиса RankService Get rank(int scores)
                    user.Rank = "";
                }
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
            var currentUser1 = topUsers.First(x => x.Id == userId);
            var currentUserIndex1 = topUsers.IndexOf(currentUser1);

            currentUser1.IsCurrentPlayer = true;

            topUsers[currentUserIndex1] = currentUser1;
            #endregion

            #region property Rank
            foreach (var user in topUsers)
            {
                // для каждого вызвать метод сервиса RankService Get rank(int scores)
                user.Rank = "";
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


            // вернуть Result<>
            return new Result<List<RateModel>>(topUsers);
        }
    }
}
