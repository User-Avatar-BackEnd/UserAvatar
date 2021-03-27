using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Api.Extentions;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Tests.Gamification
{
    public class SearchServiceTests
    {
        private readonly Mock<IUserStorage> _userStorage;
        private readonly IMapper _mapper;

        public SearchServiceTests()
        {
            _userStorage = new Mock<IUserStorage>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            _mapper = mapper;
        }
    }
}
