using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.BoardServiceTests;

public sealed class CreateBoardTests
{
    private readonly Mock<IBoardChangesService> _boardChangesService;
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly IOptions<LimitationOptions> _limitations;
    private readonly Mock<IMapper> _mapper;

    public CreateBoardTests()
    {
        _limitations = Options.Create(new LimitationOptions());
        _mapper = new Mock<IMapper>();
        _boardStorage = new Mock<IBoardStorage>();
        _boardChangesService = new Mock<IBoardChangesService>();
    }

    [Fact]
    public async Task CreateBoard_If_Max_Board_Count_Is_Reached_Returns_ResultCode_MaxBoardCount()
    {
        // Arrange
        _limitations.Value.MaxBoardCount = 10;
        _boardStorage.Setup(x => x.CountAllBoardsAsync(It.IsAny<int>())).ReturnsAsync(_limitations.Value.MaxBoardCount);

        var boardService = SetUpService();

        // Act
        var result = await boardService.CreateBoardAsync(It.IsAny<int>(), It.IsAny<string>());

        // Assert
        result.Code.Should().Be(ResultCode.MaxBoardCount);
    }

    [Fact]
    public async Task CreateBoard_If_Board_Can_Be_Created_Returns_ResultCode_Success()
    {
        // Arrage
        _limitations.Value.MaxBoardCount = 10;
        _boardStorage.Setup(x => x.CountAllBoardsAsync(It.IsAny<int>()))
            .ReturnsAsync(It.Is<int>(x => x < _limitations.Value.MaxBoardCount));

        var boardService = SetUpService();

        // Act
        var result = await boardService.CreateBoardAsync(It.IsAny<int>(), It.IsAny<string>());

        //Assert
        result.Code.Should().Be(ResultCode.Success);
        result.EventType.Should().Be(EventType.CreateBoard);
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
