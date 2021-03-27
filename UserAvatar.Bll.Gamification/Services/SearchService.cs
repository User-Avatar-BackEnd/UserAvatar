using AutoMapper;
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
        private readonly IMapper _mapper;

        public SearchService(IUserStorage userStorage,
            IMapper mapper)
        {
            _userStorage = userStorage;
            _mapper = mapper;
        }

        public async Task<PagedUsersModel> GetAllUsersAsync(int pageNumber, int pageSize, string query)
        {
            var users = await _userStorage.GetPagedUsersAsync(pageNumber, pageSize, query);

            var totalUserAmount = await _userStorage.GetUsersAmountAsync();

            var totalPages = totalUserAmount % pageSize == 0 ?
                totalUserAmount / pageSize :
                +totalUserAmount / pageSize + 1;

            var usersData =  _mapper.Map<List<User>, List<UserDataModel>>(users);

            int startedPosition = pageNumber == 1 ? 1 : ((pageNumber - 1) * pageSize) + 1;

            usersData.ForEach(x => x.Position = startedPosition++);
            
            return new PagedUsersModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalElements = totalUserAmount,
                IsFirstPage = pageNumber == 1,
                IsLastPage = pageNumber == totalPages ? true:false,
                Users = usersData
            };
        }
    }
}
