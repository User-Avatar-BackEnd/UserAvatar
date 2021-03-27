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
    public class ChangeLoginTests
    {
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mock<IMapper> _mapper;

        public ChangeLoginTests()
        {
            _userStorage = new Mock<IUserStorage>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task ChangeLogin_If_User_Is_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange 
            _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);
            
            // Act
            var result = await personalAccountService.ChangeLoginAsync(It.IsAny<int>(), It.IsAny<string>());

            // Assert
            result.Should().Be(ResultCode.NotFound);
        }

        [Fact]
        public async Task ChangeLogin_If_NewLogin_Equals_CurrentLogin_Returns_ResultCode_LoginAlreadyExist()
        {
            // Arrange
            var newLogin = "lucky_bob";

            _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User() { Login = newLogin });

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await personalAccountService.ChangeLoginAsync(It.IsAny<int>(), newLogin);

            // Assert
            result.Should().Be(ResultCode.SameLoginAsCurrent);
        }

        [Fact]
        public async Task ChangeLogin_If_NewLogin_Is_Already_Taken_Returns_ResultCode_LoginAlreadyExist()
        {
            // Arrange
            var newLogin = "lucky_bob";

            _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User() { Login = It.Is<string>(x => x != newLogin) });
            _userStorage.Setup(x => x.IsLoginExistAsync(It.IsAny<string>())).ReturnsAsync(true);

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await personalAccountService.ChangeLoginAsync(It.IsAny<int>(), newLogin);

            //Assert
            result.Should().Be(ResultCode.LoginAlreadyExist);
        }

        [Fact]
        public async Task ChangeLogin_If_Successfuly_Changed_Login_Returns_ResultCode_Success()
        {
            // Arrange
            var newLogin = "lucky_bob";

            _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User() { Login = It.Is<string>(x => x != newLogin) });
            _userStorage.Setup(x => x.IsLoginExistAsync(It.IsAny<string>())).ReturnsAsync(false);

            var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await personalAccountService.ChangeLoginAsync(It.IsAny<int>(), newLogin);

            // Assert
            result.Should().Be(ResultCode.Success);
        }
    }
}
