using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.InviteServiceTests;

public sealed class FindByQueryTests
{
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly Mock<IInviteStorage> _inviteStorage;
    private readonly Mapper _mapper;
    private readonly Mock<IUserStorage> _userStorage;
    private InviteService _service;

    public FindByQueryTests()
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
    public async Task FindByQuery_If_Query_Null_Returns_Empty_Array()
    {
        //Arrange
        _service = SetupInviteService();

        //Act
        var result = await _service.FindByQueryAsync(It.IsAny<int>(), It.IsAny<int>(), null);

        //Assert
        result.Value.Count.Should().Be(0);
    }

    [Fact]
    public async Task FindByQuery_If_User_Not_In_Board_Returns_Forbidden()
    {
        //Arrange
        var query = "query";
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        _service = SetupInviteService();

        //Act
        var result = await _service.FindByQueryAsync(It.IsAny<int>(), It.IsAny<int>(), query);

        //Assert
        result.Code.Should().Be(ResultCode.Forbidden);
    }

    [Fact]
    public async Task FindByQuery_Returns_User_List()
    {
        //Arrange
        var query = "query";
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _userStorage.Setup(x => x.InviteByQueryAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new List<User> { new(), new() });

        _service = SetupInviteService();

        //Act
        var result = await _service.FindByQueryAsync(It.IsAny<int>(), It.IsAny<int>(), query);

        //Assert
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(ResultCode.Success, result.Code);
    }
}
