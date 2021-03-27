using System.Threading.Tasks;
using AutoMapper;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.AuthServiceTests
{
    public class LogInOutTests
    {
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mock<IMapper> _mapper;

        public LogInOutTests()
        {
            _userStorage = new Mock<IUserStorage>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task Login_If_Email_Does_Not_Exist_Returns_ResultCode_InvalidEmail()
        {
            // Arrange
            _userStorage.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var authService = new AuthService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await authService.LoginAsync(It.IsNotNull<string>(), It.IsNotNull<string>());

            // Assert
            Assert.Equal(ResultCode.InvalidEmail, result.Code);
        }

        [Fact]
        public async Task Login_If_Invalid_Password_Returns_ResultCode_InvalidPassword()
        {
            // Arrange
            string password = "password";
            string invalidPassword = "invalidPassword";
            _userStorage.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User { PasswordHash = PasswordHash.CreateHash(invalidPassword) });

            var authService = new AuthService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await authService.LoginAsync(It.IsNotNull<string>(), password);

            // Assert
            Assert.Equal(ResultCode.InvalidPassword, result.Code);
        }

        [Fact]
        public async Task Login_If_Successfully_Returns_ResultCode_Success_And_EventType_Login()
        {
            // Arrange
            string password = "password";
            _userStorage.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User { PasswordHash = PasswordHash.CreateHash(password) });

            var authService = new AuthService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await authService.LoginAsync(It.IsNotNull<string>(), password);

            // Assert
            Assert.Equal(ResultCode.Success, result.Code);
            Assert.Equal(EventType.Login, result.EventType);
        }

        [Fact]
        public void Logout_Returns_EventType_Logout()
        {
            // Arrange
            var authService = new AuthService(_userStorage.Object, _mapper.Object);

            // Act
            var result = authService.Logout();

            // Assert
            Assert.Equal(EventType.Logout, result);
        }
    }
}
