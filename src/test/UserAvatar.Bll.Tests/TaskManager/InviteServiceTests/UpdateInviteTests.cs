using System.Threading.Tasks;
using AutoMapper;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.InviteServiceTests;

public sealed class UpdateInviteTests
{
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly Mock<IInviteStorage> _inviteStorage;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IUserStorage> _userStorage;
    private InviteService _service;

    public UpdateInviteTests()
    {
        _inviteStorage = new Mock<IInviteStorage>();
        _mapper = new Mock<IMapper>();
        _userStorage = new Mock<IUserStorage>();
        _boardStorage = new Mock<IBoardStorage>();
    }

    private InviteService SetupInviteService()
    {
        return new InviteService(
            _inviteStorage.Object,
            _mapper.Object,
            _userStorage.Object,
            _boardStorage.Object);
    }

    [Fact]
    public async Task UpdateInvite_If_Invite_Not_Exist_Return_NotFound()
    {
        //Arrange
        _inviteStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Invite)null);
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User());
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        _service = SetupInviteService();

        //Act
        var result = await _service.UpdateInviteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        //Assert
        Assert.Equal(ResultCode.NotFound, result);
    }

    [Fact]
    public async Task UpdateInvite_If_User_Not_Exist_Return_NotFound()
    {
        //Arrange
        _inviteStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Invite { Status = InviteStatus.Pending });
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((User)null);
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);
        _service = SetupInviteService();

        //Act
        var result = await _service.UpdateInviteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        //Assert
        Assert.Equal(ResultCode.NotFound, result);
    }

    [Fact]
    public async Task UpdateInvite_If_Status_Accepted_Return_NotFound()
    {
        //Arrange
        _inviteStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Invite { Status = InviteStatus.Accepted });
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User());
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        _service = SetupInviteService();

        //Act
        var result = await _service.UpdateInviteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        //Assert
        Assert.Equal(ResultCode.NotFound, result);
    }

    [Fact]
    public async Task UpdateInvite_If_User_Member_Return_NotFound()
    {
        //Arrange
        _inviteStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Invite { Status = InviteStatus.Pending });
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User());
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);
        _service = SetupInviteService();

        //Act
        var result = await _service.UpdateInviteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        //Assert
        Assert.Equal(ResultCode.NotFound, result);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(9)]
    public async Task UpdateInvite_If_User_Not_Invited_User_Return_Forbidden(int invitedId)
    {
        //Arrange
        _inviteStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Invite { InvitedId = invitedId, Status = InviteStatus.Pending });
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User());
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        _service = SetupInviteService();

        //Act
        var result = await _service.UpdateInviteAsync(It.IsAny<int>(), It.Is<int>(x => x < 10), It.IsAny<int>());

        //Assert
        Assert.Equal(ResultCode.Forbidden, result);
    }

    [Fact]
    public async Task UpdateInvite_If_StatusCode_Accepted_Returns_Success()
    {
        //Arrange
        _inviteStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Invite { Status = InviteStatus.Pending });
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User());
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        _service = SetupInviteService();

        //Act
        var result = await _service.UpdateInviteAsync(It.IsAny<int>(), It.Is<int>(x => x < 10), InviteStatus.Accepted);

        //Assert
        Assert.Equal(ResultCode.Success, result);
    }
}
