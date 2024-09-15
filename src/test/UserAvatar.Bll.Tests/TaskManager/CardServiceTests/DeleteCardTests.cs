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

public sealed class DeleteCardTests
{
    private readonly Mock<IBoardChangesService> _boardChangesService;
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly Mock<ICardStorage> _cardStorage;
    private readonly Mock<IColumnStorage> _columnStorage;
    private readonly IOptions<LimitationOptions> _limitations;
    private readonly Mock<IMapper> _mapper;

    public DeleteCardTests()
    {
        _mapper = new Mock<IMapper>();
        _columnStorage = new Mock<IColumnStorage>();
        _boardStorage = new Mock<IBoardStorage>();
        _boardChangesService = new Mock<IBoardChangesService>();
        _cardStorage = new Mock<ICardStorage>();

        _limitations = Options.Create(new LimitationOptions { MaxCardCount = 100 });
    }

    [Fact]
    public async Task DeleteCard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
    {
        // Arrange
        _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Card)null);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.DeleteCardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.NotFound, result);
    }

    [Fact]
    public async Task DeleteCard_If_Not_User_Board_Returns_ResultCode_NotFound()
    {
        // Arrange
        _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Card());

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.DeleteCardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.Forbidden, result);
    }

    [Fact]
    public async Task DeleteCard_If_Success_Deleted_Returns_ResultCode_Success()
    {
        // Arrange
        _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Card());

        _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        var cardService = SetupCardService();

        // Act
        var result = await cardService.DeleteCardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        Assert.Equal(ResultCode.Success, result);
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
