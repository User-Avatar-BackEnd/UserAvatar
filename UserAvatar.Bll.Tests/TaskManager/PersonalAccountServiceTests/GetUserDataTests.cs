using AutoMapper;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.PersonalAccountServiceTests
{
    public class GetUserDataTests
    {
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mock<IMapper> _mapper;

        public GetUserDataTests()
        {
            _userStorage = new Mock<IUserStorage>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetUsersData_If_User_Is_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await personalAccountService.GetUsersDataAsync(It.IsAny<int>());

            // Assert
            result.Code.Should().Be(ResultCode.NotFound);
        }

        [Fact]
        public async Task GetUsersData_If_User_Is_Exist_Returns_ResultCode_Success()
        {
            // Arrange
            _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await personalAccountService.GetUsersDataAsync(It.IsAny<int>());

            // Assert
            result.Code.Should().Be(ResultCode.Success);
        }
    }
}
