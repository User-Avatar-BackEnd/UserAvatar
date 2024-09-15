using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.BoardServiceTests;

public sealed class GetAllBoardsTests
{
    private readonly Mock<IBoardChangesService> _boardChangesService;
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly IOptions<LimitationOptions> _limitations;
    private readonly IMapper _mapper;

    public GetAllBoardsTests()
    {
        _limitations = Options.Create(new LimitationOptions());

        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            _mapper = mapper;
        }

        _boardStorage = new Mock<IBoardStorage>();
        _boardChangesService = new Mock<IBoardChangesService>();
    }

    [Fact]
    public async Task GetAllBoards_If_User_Has_No_Boards_Returns_Empty_List()
    {
        // Arrange
        _boardStorage.Setup(x => x.GetAllBoardsAsync(It.IsAny<int>())).ReturnsAsync(new List<Board>());

        var boardService = SetUpService();

        // Act
        var result = await boardService.GetAllBoardsAsync(It.IsAny<int>());

        // Assert
        result.Value.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetAllBoards_If_User_Has_Boards_Returns_Boards_List()
    {
        // Arrange
        _boardStorage.Setup(x => x.GetAllBoardsAsync(It.IsAny<int>())).ReturnsAsync(
            new List<Board> { new(), new(), new() });

        var boardService = SetUpService();

        // Act
        var result = await boardService.GetAllBoardsAsync(It.IsAny<int>());

        // Assert
        result.Value.Count().Should().Be(3);
    }

    private BoardService SetUpService()
    {
        return new BoardService(
            _boardStorage.Object,
            _mapper,
            _limitations,
            _boardChangesService.Object);
    }
}
