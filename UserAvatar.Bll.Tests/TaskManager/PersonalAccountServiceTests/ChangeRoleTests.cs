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
    public class ChangeRoleTests
    {
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mock<IMapper> _mapper;

        public ChangeRoleTests()
        {
            _userStorage = new Mock<IUserStorage>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task ChangeRole_If_User_Is_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _userStorage.Setup(x => x.GetByLoginAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await personalAccountService.ChangeRoleAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            result.Should().Be(ResultCode.NotFound);
        }

        // Real trouble with test name
        [Fact]
        public async Task ChangeRole_If_User_Is_UserToChangeRole_Returns_ResultCode_Forbidden()
        {
            // Arrange
            var userId = 1;

            _userStorage.Setup(x => x.GetByLoginAsync(It.IsAny<string>())).ReturnsAsync(new User()
            {
                Id = userId
            });

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await personalAccountService.ChangeRoleAsync(userId, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            result.Should().Be(ResultCode.Forbidden);
        }

        [Fact]
        public async Task ChangeRole_If_Successfully_Changed_User_Role_Returns_ResultCode_Success()
        {
            // Arrange
            var userId = 1;

            _userStorage.Setup(x => x.GetByLoginAsync(It.IsAny<string>())).ReturnsAsync(new User()
            {
                Id = It.Is<int>(x => x != userId)
            });

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await personalAccountService.ChangeRoleAsync(userId, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            result.Should().Be(ResultCode.Success);
        }
    }
}
