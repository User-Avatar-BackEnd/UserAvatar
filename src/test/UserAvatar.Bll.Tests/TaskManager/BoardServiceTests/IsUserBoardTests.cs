using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.BoardServiceTests;

public sealed class IsUserBoardTests
{
    private readonly Mock<IBoardChangesService> _boardChangesService;
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly IOptions<LimitationOptions> _limitations;
    private readonly Mock<IMapper> _mapper;

    public IsUserBoardTests()
    {
        _limitations = Options.Create(new LimitationOptions());
        _mapper = new Mock<IMapper>();
        _boardStorage = new Mock<IBoardStorage>();
        _boardChangesService = new Mock<IBoardChangesService>();
    }

    [Fact]
    public async Task IsUserBoard_If_Users_Board_Returns_True()
    {
        // Arrange
        _boardStorage.Setup(method => method.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        var boardService = SetUpService();

        // Act
        var result = await boardService.IsUserBoardAsync(4, 2);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async Task IsUserBoard_If_Not_Users_Board_Returns_False()
    {
        // Arrange
        var userId = 1;
        var boardId = 1;

        _boardStorage.Setup(method => method.IsUserBoardAsync(userId, boardId)).ReturnsAsync(true);

        var boardService = SetUpService();

        // Act
        var result1 = await boardService.IsUserBoardAsync(3, boardId);
        var result2 = await boardService.IsUserBoardAsync(userId, 3);
        var result3 = await boardService.IsUserBoardAsync(3, 3);

        // Assert
        result1.Should().Be(false);
        result2.Should().Be(false);
        result3.Should().Be(false);
    }

    private BoardService SetUpService()
    {
        return new BoardService(
            _boardStorage.Object,
            _mapper.Object,
            _limitations,
            _boardChangesService.Object);
    }
}
