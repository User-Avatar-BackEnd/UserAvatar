using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services.Interfaces;
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

        //public async Task<List<RateModel>> GetTopRate(int userId)
        //{ 
        
        //}
    }
}
