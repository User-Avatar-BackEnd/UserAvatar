using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.CardServiceTests;

public sealed class UpdateCardTests
{
    private readonly Mock<IBoardChangesService> _boardChangesService;
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly Mock<ICardStorage> _cardStorage;
    private readonly Mock<IColumnStorage> _columnStorage;
    private readonly IOptions<LimitationOptions> _limitations;
    private readonly Mock<IMapper> _mapper;

    public UpdateCardTests()
    {
        _mapper = new Mock<IMapper>();
        _columnStorage = new Mock<IColumnStorage>();
        _boardStorage = new Mock<IBoardStorage>();
        _boardChangesService = new Mock<IBoardChangesService>();
        _cardStorage = new Mock<ICardStorage>();

        _limitations = Options.Create(new LimitationOptions { MaxCardCount = 100 });
    }

    [Fact]
    public async Task UpdateCard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
    {
        // Arrange
        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
            .ReturnsAsync(false);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.UpdateCardAsync(It.IsAny<CardModel>(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.NotFound, result.Code);
    }

    [Fact]
    public async Task UpdateCard_If_Board_Does_Not_Have_Column_Returns_ResultCode_Foridden()
    {
        // Arrange
        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.UpdateCardAsync(new CardModel(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.Forbidden, result.Code);
    }

    [Fact]
    public async Task UpdateCard_If_Card_Does_Not_Exist_Returns_ResultCode_NotFound()
    {
        // Arrange
        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Card)null);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.UpdateCardAsync(new CardModel(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.NotFound, result.Code);
    }

    [Fact]
    public async Task UpdateCard_If_Not_User_Board_Returns_ResultCode_Forbidden()
    {
        // Arrange
        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Card());

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.UpdateCardAsync(new CardModel(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.Forbidden, result.Code);
    }

    [Fact]
    public async Task UpdateCard_If_Status_Not_Changed_Returns_EventType_Null()
    {
        // Arrange
        var oldColumnId = 1;

        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Card { ColumnId = oldColumnId });

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.UpdateCardAsync(
            new CardModel { ColumnId = oldColumnId },
            It.IsAny<int>(),
            It.IsAny<int>());

        // Assert
        //Assert.Equal(ResultCode.Success, result.Code);
        Assert.Null(result.EventType);
    }

    [Fact]
    public async Task UpdateCard_If_Status_Changed_By_Owner_Returns_EventType_ChangeCardStatusOnOwnBoard()
    {
        // Arrange
        var oldColumnId = 1;
        var userId = 1;

        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Card { ColumnId = oldColumnId });

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(new Board { OwnerId = userId });

        var cardService = SetupCardService();

        // Act
        var result = await cardService.UpdateCardAsync(
            new CardModel { ColumnId = oldColumnId + 1 },
            It.IsAny<int>(),
            userId);

        // Assert
        Assert.Equal(ResultCode.Success, result.Code);
        Assert.Equal(EventType.ChangeCardStatusOnOwnBoard, result.EventType);
    }

    [Fact]
    public async Task UpdateCard_If_Status_Changed_By_Member_Returns_EventType_ChangeCardStatusOnAlienBoard()
    {
        // Arrange
        var oldColumnId = 1;
        var userId = 1;

        _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Card { ColumnId = oldColumnId });

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(new Board { OwnerId = userId + 1 });

        var cardService = SetupCardService();

        // Act
        var result = await cardService.UpdateCardAsync(
            new CardModel { ColumnId = oldColumnId + 1 },
            It.IsAny<int>(),
            userId);

        // Assert
        Assert.Equal(ResultCode.Success, result.Code);
        Assert.Equal(EventType.ChangeCardStatusOnAlienBoard, result.EventType);
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
