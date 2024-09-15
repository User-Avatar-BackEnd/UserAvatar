using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.PersonalAccountServiceTests;

public sealed class ChangePasswordTests
{
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IUserStorage> _userStorage;

    public ChangePasswordTests()
    {
        _userStorage = new Mock<IUserStorage>();
        _mapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task ChangePassword_If_User_Is_Not_Exist_Returns_ResultCode_NotFound()
    {
        // Arrange
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

        var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

        // Act
        var result =
            await personalAccountService.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>());

        // Assert
        result.Should().Be(ResultCode.NotFound);
    }

    [Fact]
    public async Task
        ChangePassword_If_OldPassword_Does_Not_Equals_Current_Password_Returns_ResultCode_InvalidPassword()
    {
        // Arrange
        var oldPassword = "vErY_SeCrEt_pAsSwOrD";
        var currentPassword = "SeCrEt_pAsSwOrD";

        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User
        {
            PasswordHash = PasswordHash.CreateHash(currentPassword)
        });

        var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

        // Act
        var result = await personalAccountService.ChangePasswordAsync(It.IsAny<int>(), oldPassword, It.IsAny<string>());

        // Assert
        result.Should().Be(ResultCode.InvalidPassword);
    }

    [Fact]
    public async Task ChangePassword_If_NewPassword_Equals_OldPassword_Returns_ResultCode_SamePasswordAsOld()
    {
        // Arrange
        var oldPassword = "vErY_SeCrEt_pAsSwOrD";

        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User
        {
            PasswordHash = PasswordHash.CreateHash(oldPassword)
        });

        var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

        // Act
        var result = await personalAccountService.ChangePasswordAsync(It.IsAny<int>(), oldPassword, oldPassword);

        // Assert
        result.Should().Be(ResultCode.SamePasswordAsOld);
    }

    [Fact]
    public async Task ChangePassword_If_Successful_Password_Changed_Attempt_Returns_ResultCode_Success()
    {
        // Arrange
        var oldPassword = "vErY_SeCrEt_pAsSwOrD";
        var newPassword = "vErY_vErY_SeCrEt_pAsSwOrD";

        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User
        {
            PasswordHash = PasswordHash.CreateHash(oldPassword)
        });

        var personalAccountService = new PersonalAccountService(_userStorage.Object, _mapper.Object);

        // Act
        var result = await personalAccountService.ChangePasswordAsync(It.IsAny<int>(), oldPassword, newPassword);

        // Assert
        result.Should().Be(ResultCode.Success);
    }
}
