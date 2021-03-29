using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.Gamification.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.Gamification
{
    public class SearchServiceTests
    {
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mapper _mapper;
        private SearchService _searchService;
        public SearchServiceTests()
        {
            _userStorage = new Mock<IUserStorage>();
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            _mapper = new Mapper(configuration);
        }

        private SearchService SetupService()
        {
            return new(_userStorage.Object, _mapper);
        }

        [Fact]
        public async Task GetAllUsers_Throws_FullList()
        {
            //Arrange
            var query = "";
            var pageSize = 1;
            var expectedUserCollection = new List<User> {new(), new(), new()};
            _userStorage.Setup(x => x.GetPagedUsersAsync(It.IsAny<int>(), pageSize, query))
                .ReturnsAsync(new List<User> {new(), new(), new()});
            _userStorage.Setup(x => x.GetUsersAmountAsync())
                .ReturnsAsync(expectedUserCollection.Count);
            
            _searchService = SetupService();
            
            //Act
            var result = 
                await _searchService.GetAllUsersAsync(It.IsAny<int>(), pageSize, query);
            
            //Assert
            result.Users.Count.Should().Be(expectedUserCollection.Count);
        }
        [Fact]
        public async Task GetAllUsers_If_PageSize_Less_Zero_Throws_DividedByZero()
        {
            //Arrange
            var query = "";
            var expectedUserCollection = new List<User> {new(), new(), new()};
            _userStorage.Setup(x => x.GetPagedUsersAsync(It.IsAny<int>(), It.Is<int>(x=> x<1), query))
                .ReturnsAsync(new List<User> {new(), new(), new()});
            _userStorage.Setup(x => x.GetUsersAmountAsync())
                .ReturnsAsync(expectedUserCollection.Count);
            
            _searchService = SetupService();
            
            //Assert
            await Assert.ThrowsAsync<System.DivideByZeroException>(() =>
                _searchService.GetAllUsersAsync(It.IsAny<int>(), It.Is<int>(x => x < 1), query));
        }
    }
}
