using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mock<IMapper> _mapper;

        public AuthServiceTests()
        {
            _userStorage = new Mock<IUserStorage>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task Registration_If_User_With_This_Email_Alrady_Exist_Returns_ResultCode_EmailAlreadyExist()
        {
            // Arrange
            _userStorage.Setup(x => x.IsUserExistAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var authService = new AuthService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await authService.RegisterAsync(It.IsNotNull<string>(), It.IsAny<string>(), It.IsNotNull<string>());

            // Assert
            Assert.Equal(ResultCode.EmailAlreadyExist, result.Code);
        }

        [Fact]
        public async Task Registration_If_User_With_This_Login_Alrady_Exist_Returns_ResultCode_LoginAlreadyExist()
        {
            // Arrange
            var login = "login";
            _userStorage.Setup(x => x.IsUserExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _userStorage.Setup(x => x.IsLoginExistAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var authService = new AuthService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await authService.RegisterAsync(It.IsNotNull<string>(), login, It.IsNotNull<string>());

            // Assert
            Assert.Equal(ResultCode.LoginAlreadyExist, result.Code);
        }

        [Fact]
        public async Task Registration_If_Successfully_Returns_ResultCode_Success_And_EventType_Register()
        {
            // Arrange
            var password = "password";
            _userStorage.Setup(x => x.IsUserExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _userStorage.Setup(x => x.IsLoginExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var authService = new AuthService(_userStorage.Object, _mapper.Object);

            // Act
            var result = await authService.RegisterAsync(It.IsNotNull<string>(), It.IsAny<string>(), password);

            // Assert
            Assert.Equal(ResultCode.Success, result.Code);
            Assert.Equal(EventType.Registration, result.EventType);
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
