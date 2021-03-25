using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Gamification.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUserStorage _userStorage;
        private readonly IRankService _rankService;
        private readonly IMapper _mapper;

        public SearchService(IUserStorage userStorage,
            IRankService rankService,
            IMapper mapper)
        {
            _userStorage = userStorage;
            _rankService = rankService;
            _mapper = mapper;
        }

        /*
          public int Position { get; set; }+

        public string Rank { get; set; } -

        public string Login { get; set; } +

        public int Score { get; set; } +

        public string Role { get; set; } +
        
         */

        public async Task GetAllUsers(int pageNumber, int pageSize)
        {
            var users = await _userStorage.GetPagedUsersAsync(pageNumber, pageSize);

            var usersData =  _mapper.Map<List<User>, List<UserDataModel>>(users);

            // todo : call the method and give it the list

           // usersData.ForEach(async x => x.Rank = await _rankService.GetRank(x.Score).);



                var pagedUserModel = new PagedUsersModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
             // TotalPages = 
             // TotalElements = 
                IsFirstPage = pageNumber == 1,
             // IsLastPage = 
             // List<UserDataModel> Users = userData;
            };
        }
    }
}
