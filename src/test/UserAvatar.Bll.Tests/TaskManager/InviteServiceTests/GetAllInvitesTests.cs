using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.InviteServiceTests;

public sealed class GetAllInvitesTests
{
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly Mock<IInviteStorage> _inviteStorage;
    private readonly Mapper _mapper;
    private readonly Mock<IUserStorage> _userStorage;
    private InviteService _service;

    public GetAllInvitesTests()
    {
        _inviteStorage = new Mock<IInviteStorage>();

        var myProfile = new MappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        _mapper = new Mapper(configuration);

        _userStorage = new Mock<IUserStorage>();
        _boardStorage = new Mock<IBoardStorage>();
    }

    private InviteService SetupInviteService()
    {
        return new InviteService(
            _inviteStorage.Object,
            _mapper,
            _userStorage.Object,
            _boardStorage.Object);
    }

    [Fact]
    public async Task GetAllInvites_If_No_User_Returns_NotFound()
    {
        //Arrange
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((User)null);
        _service = SetupInviteService();

        //Act
        var result = await _service.GetAllInvitesAsync(It.IsAny<int>());

        //Assert
        Assert.Equal(ResultCode.NotFound, result.Code);
    }

    [Fact]
    public async Task GetAllInvites_Returns_InviteList()
    {
        //Arrange
        _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User());
        _inviteStorage.Setup(x => x.GetInvitesAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Invite> { new(), new() });
        _service = SetupInviteService();

        //Act
        var result = await _service.GetAllInvitesAsync(It.IsAny<int>());

        //Assert
        Assert.Equal(ResultCode.Success, result.Code);
        Assert.Equal(2, result.Value.Count);
    }
}
