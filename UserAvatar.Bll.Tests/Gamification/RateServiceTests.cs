using AutoMapper;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.Gamification
{
    public class RateServiceTests
    {
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mock<IRankService> _rankService;
        private readonly IMapper _mapper;

        public RateServiceTests()
        {
            _userStorage = new Mock<IUserStorage>();
            _rankService = new Mock<IRankService>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            _mapper = mapper;
        }

        [Fact]
        public async Task GetTopRateAsync_If_User_In_Top_Returns_Rate() // 10 users and and you are 10 user
        {
            // Arrange
            var currentUserId = 10;

            _userStorage.Setup(x => x.GetUsersRateAsync()).ReturnsAsync(new List<User>() {
                new User() { Id = 1, Score = 100 },
                new User() { Id = 2, Score = 90 },
                new User() { Id = 3, Score = 80 },
                new User() { Id = 4, Score = 75 },
                new User() { Id = 5, Score = 60 },
                new User() { Id = 6, Score = 50 },
                new User() { Id = 7, Score = 49 },
                new User() { Id = 8, Score = 20 },
                new User() { Id = 9, Score = 10 },
                new User() { Id = 10, Score = 4 },
            });
            _rankService.Setup(x => x.GetAllRanksDataAsync(It.IsAny<int>())).ReturnsAsync(new RankDataModel()
            {
                Name = "Mentor",
                Score = 100,
                MaxScores = 200
            });

            var rateService = SetUpService();

            // Act
            var result = await rateService.GetTopRateAsync(currentUserId);

            // Assert
            result.Code.Should().Be(ResultCode.Success);
            result.Value.TopUsers.Count.Should().Be(10);
            result.Value.UnderTopUsers.Count.Should().Be(0);
            result.Value.TopUsers[9].IsCurrentPlayer.Should().BeTrue();
        }

        [Fact]
        public async Task GetTopRateAsync_If_User_Not_In_Top_Returns_Rate() //// 14 users and and you are 14 user
        {
            // Arrange
            var currentUserId = 14;

            _userStorage.Setup(x => x.GetUsersRateAsync()).ReturnsAsync(new List<User>() {
                new User() { Id = 1, Score = 100 },
                new User() { Id = 2, Score = 39 },
                new User() { Id = 3, Score = 80 },
                new User() { Id = 10, Score = 15 },
                new User() { Id = 5, Score = 30 },
                new User() { Id = 6, Score = 50 },
                new User() { Id = 7, Score = 49 },
                new User() { Id = 8, Score = 20 },
                new User() { Id = 9, Score = 230 },
                new User() { Id = 11, Score = 140 },
                new User() { Id = 13, Score = 140 },
                new User() { Id = 14, Score = 5 },
                new User() { Id = 12, Score = 140 },
            });
            _rankService.Setup(x => x.GetAllRanksDataAsync(It.IsAny<int>())).ReturnsAsync(new RankDataModel()
            {
                Name = "Master",
                Score = 100,
                MaxScores = 200
            });

            var rateService = SetUpService();

            // Act
            var result = await rateService.GetTopRateAsync(currentUserId);

            // Assert
            result.Code.Should().Be(ResultCode.Success);
            result.Value.TopUsers.Count.Should().Be(10);
            result.Value.UnderTopUsers.Count.Should().Be(3);
            result.Value.UnderTopUsers[1].IsCurrentPlayer.Should().BeTrue();
        }

        [Fact]
        public async Task GetTopRateAsync_If_User_The_Last_In_Users_List_Returns_Rate() // 15 users and you are 13 in rate user
        {
            // Arrange
            var currentUserId = 13;

            _userStorage.Setup(x => x.GetUsersRateAsync()).ReturnsAsync(new List<User>() {
                new User() { Id = 1, Score = 100 },
                new User() { Id = 2, Score = 90 },
                new User() { Id = 3, Score = 80 },
                new User() { Id = 4, Score = 75 },
                new User() { Id = 5, Score = 56 },
                new User() { Id = 6, Score = 50 },
                new User() { Id = 7, Score = 49 },
                new User() { Id = 8, Score = 40 },
                new User() { Id = 9, Score = 31 },
                new User() { Id = 10, Score = 30 },
                new User() { Id = 12, Score = 20 },
                new User() { Id = 13, Score = 15},
                new User() { Id = 14, Score = 5 },
            });
            _rankService.Setup(x => x.GetAllRanksDataAsync(It.IsAny<int>())).ReturnsAsync(new RankDataModel()
            {
                Name = "Boom",
                Score = 100,
                MaxScores = 200
            });

            var rateService = SetUpService();

            // Act
            var result = await rateService.GetTopRateAsync(currentUserId);

            // Assert
            result.Code.Should().Be(ResultCode.Success);
            result.Value.TopUsers.Count.Should().Be(10);
            result.Value.UnderTopUsers.Count.Should().Be(3);
            result.Value.UnderTopUsers[1].IsCurrentPlayer.Should().BeTrue();
            result.Value.UnderTopUsers[1].Id.Should().Be(13);
        }

        [Fact]
        public async Task GetTopRateAsync_If_UsesList_Has_Less_Than_TopTen_Returns_Rate() // 5 users only
        {
            // Arrange
            var currentUserId = 3;

            _userStorage.Setup(x => x.GetUsersRateAsync()).ReturnsAsync(new List<User>() {
                new User() { Id = 1, Score = 100 },
                new User() { Id = 3, Score = 80 },
                new User() { Id = 4, Score = 50 },
                new User() { Id = 2, Score = 39 },
                new User() { Id = 5, Score = 30 },
            });
            _rankService.Setup(x => x.GetAllRanksDataAsync(It.IsAny<int>())).ReturnsAsync(new RankDataModel()
            {
                Name = "Student",
                Score = 100,
                MaxScores = 200
            });

            var rateService = SetUpService();

            // Act
            var result = await rateService.GetTopRateAsync(currentUserId);

            // Assert
            result.Code.Should().Be(ResultCode.Success);
            result.Value.TopUsers.Count.Should().Be(5);
            result.Value.UnderTopUsers.Count.Should().Be(0);
            result.Value.TopUsers[1].IsCurrentPlayer.Should().BeTrue();
        }

        private RateService SetUpService()
        {
            return new RateService(
                _userStorage.Object,
                _rankService.Object,
                _mapper);
        }
    }
}
