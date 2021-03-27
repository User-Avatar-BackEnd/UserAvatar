using AutoMapper;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Services;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.Gamification
{
    public class HistoryServiceTests
    {
        private readonly Mock<IHistoryStorage> _historyStorage;
        private readonly Mock<IEventStorage> _eventStorage;
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mock<IMapper> _mapper;

        public HistoryServiceTests()
        {
            _mapper = new Mock<IMapper>();
            _historyStorage = new Mock<IHistoryStorage>();
            _eventStorage = new Mock<IEventStorage>();
            _userStorage = new Mock<IUserStorage>();
        }

        [Fact]
        public async Task GetHistory_If_User_Is_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _userStorage.Setup(x => x.GetByLoginAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var historyService = SetUpService();

            // Act
            var result = await historyService.GetHistoryAsync(It.IsAny<string>());

            // Assert
            result.Code.Should().Be(ResultCode.NotFound);
        }

        [Fact]
        public async Task GetHistory_If_User_Is_Exist_Returns_ResultCode_Success()
        {
            // Arrange
            _userStorage.Setup(x => x.GetByLoginAsync(It.IsAny<string>())).ReturnsAsync(new User());

            var historyService = SetUpService();

            // Act
            var result = await historyService.GetHistoryAsync(It.IsAny<string>());

            // Assert
            result.Code.Should().Be(ResultCode.Success);
        }

        private HistoryService SetUpService()
        {
            return new HistoryService(
                _historyStorage.Object,
                _userStorage.Object,
                _eventStorage.Object,
                _mapper.Object);
        }
    }
}
