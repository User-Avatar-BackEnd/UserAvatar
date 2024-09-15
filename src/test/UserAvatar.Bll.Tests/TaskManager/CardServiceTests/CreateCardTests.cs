using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.CardServiceTests;

public sealed class CreateCardTests
{
    private readonly Mock<IBoardChangesService> _boardChangesService;
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly Mock<ICardStorage> _cardStorage;
    private readonly Mock<IColumnStorage> _columnStorage;
    private readonly IOptions<LimitationOptions> _limitations;
    private readonly Mock<IMapper> _mapper;

    public CreateCardTests()
    {
        _mapper = new Mock<IMapper>();
        _columnStorage = new Mock<IColumnStorage>();
        _boardStorage = new Mock<IBoardStorage>();
        _boardChangesService = new Mock<IBoardChangesService>();
        _cardStorage = new Mock<ICardStorage>();

        _limitations = Options.Create(new LimitationOptions { MaxCardCount = 100 });
    }

    [Fact]
    public async Task CreateCard_If_Column_Does_Not_Exist_Returns_ResultCode_NotFound()
    {
        // Arrange
        _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Column)null);

        var cardService = SetupCardService();

        // Act
        var result =
            await cardService.CreateCardAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.NotFound, result.Code);
    }

    [Fact]
    public async Task CreateCard_If_Board_Does_Not_Contains_Column_Returns_ResultCode_NotFound()
    {
        // Arrange
        var boardId = 1;

        _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Column { BoardId = boardId + 1 });

        var cardService = SetupCardService();

        // Act
        var result = await cardService.CreateCardAsync(It.IsAny<string>(), boardId, It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.NotFound, result.Code);
    }

    [Fact]
    public async Task CreateCard_If_Not_User_Board_Returns_ResultCode_Forbidden()
    {
        // Arrange
        var boardId = 1;

        _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Column { BoardId = boardId });

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), boardId))
            .ReturnsAsync(false);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.CreateCardAsync(It.IsAny<string>(), boardId, It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.Forbidden, result.Code);
    }

    [Fact]
    public async Task CreateCard_If_Max_Card_Count_In_Column_Returns_ResultCode_MaxCardCount()
    {
        // Arrange
        var boardId = 1;

        _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Column { BoardId = boardId });

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), boardId))
            .ReturnsAsync(true);

        _cardStorage.Setup(x => x.GetCardsCountInColumnAsync(It.IsAny<int>()))
            .ReturnsAsync(_limitations.Value.MaxCardCount);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.CreateCardAsync(It.IsAny<string>(), boardId, It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.MaxCardCount, result.Code);
    }

    [Fact]
    public async Task CreateCard_Card_Created_By_Board_Owner_Returns_EventType_CreateCardOnOwnBoard()
    {
        // Arrange
        var boardId = 1;
        var userId = 1;

        _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Column { BoardId = boardId });

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), boardId))
            .ReturnsAsync(true);

        _cardStorage.Setup(x => x.GetCardsCountInColumnAsync(It.IsAny<int>()))
            .ReturnsAsync(_limitations.Value.MaxCardCount - 1);

        _boardStorage.Setup(x => x.GetBoardAsync(boardId))
            .ReturnsAsync(new Board { OwnerId = userId });

        var cardService = SetupCardService();

        // Act
        var result = await cardService.CreateCardAsync(It.IsAny<string>(), boardId, It.IsAny<int>(), userId);

        // Assert
        Assert.Equal(ResultCode.Success, result.Code);
        Assert.Equal(EventType.CreateCardOnOwnBoard, result.EventType);
    }

    [Fact]
    public async Task CreateCard_Card_Created_By_Member_Returns_EventType_CreateCardOnAlienBoard()
    {
        // Arrange
        var boardId = 1;
        var userId = 1;

        _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Column { BoardId = boardId });

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), boardId))
            .ReturnsAsync(true);

        _cardStorage.Setup(x => x.GetCardsCountInColumnAsync(It.IsAny<int>()))
            .ReturnsAsync(_limitations.Value.MaxCardCount - 1);

        _boardStorage.Setup(x => x.GetBoardAsync(boardId))
            .ReturnsAsync(new Board { OwnerId = userId + 1 });

        var cardService = SetupCardService();

        // Act
        var result = await cardService.CreateCardAsync(It.IsAny<string>(), boardId, It.IsAny<int>(), userId);

        // Assert
        Assert.Equal(ResultCode.Success, result.Code);
        Assert.Equal(EventType.CreateCardOnAlienBoard, result.EventType);
    }

    private CardService SetupCardService()
    {
        return new CardService(
            _cardStorage.Object,
            _mapper.Object,
            _boardStorage.Object,
            _columnStorage.Object,
            _limitations,
            _boardChangesService.Object);
    }
}
