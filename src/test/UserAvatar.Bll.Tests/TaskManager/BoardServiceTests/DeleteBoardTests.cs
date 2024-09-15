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

public sealed class DeleteBoardTests
{
    private readonly Mock<IBoardChangesService> _boardChangesService;
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly IOptions<LimitationOptions> _limitations;
    private readonly Mock<IMapper> _mapper;

    public DeleteBoardTests()
    {
        _limitations = Options.Create(new LimitationOptions());
        _mapper = new Mock<IMapper>();
        _boardStorage = new Mock<IBoardStorage>();
        _boardChangesService = new Mock<IBoardChangesService>();
    }

    [Fact]
    public async Task DeleteBoard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
    {
        // Arrange
        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>())).ReturnsAsync(false);

        var boardService = SetUpService();

        //Act
        var result = await boardService.DeleteBoardAsync(It.IsAny<int>(), It.IsAny<int>());

        //Assert
        result.Should().Be(ResultCode.NotFound);
    }

    [Fact]
    public async Task DeleteBoard_If_Is_Not_Member_Or_User_Returns_ResultCode_Forbidden()
    {
        // Arrange
        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        _boardStorage.Setup(x => x.IsOwnerBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

        var boardService = SetUpService();

        // Act
        var result = await boardService.DeleteBoardAsync(It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.Should().Be(ResultCode.Forbidden);
    }

    [Fact]
    public async Task DeleteBoard_If_Is_Member_Or_User_Returns_ResultCode_Succcess()
    {
        // Arrange
        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        _boardStorage.Setup(x => x.IsOwnerBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        var boardService = SetUpService();

        // Act
        var result = await boardService.DeleteBoardAsync(It.IsAny<int>(), It.IsAny<int>());

        //Assert
        result.Should().Be(ResultCode.Success);
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
