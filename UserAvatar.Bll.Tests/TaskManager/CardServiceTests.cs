using System;
using AutoMapper;
using Moq;
using UserAvatar.Api.Extentions;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager
{
    //public class CardServiceTests
    //{
    //    private readonly ICardStorage _cardStorage;
    //    private readonly IBoardStorage _boardStorage;
    //    private readonly IColumnStorage _columnStorage;
    //    private readonly LimitationOptions _limitations;
    //    private readonly IMapper _mapper;
    //    private readonly IBoardChangesService _boardChangesService;

    //    public CardServiceTests(IMapper mapper)
    //    {
    //        _mapper = mapper;
    //    }

    //    public async Task<Result<CardModel>> CreateCardAsync(string title,
    //int boardId, int columnId, int userId)
    //    {
    //        var column = await _columnStorage.GetColumnByIdAsync(columnId);

    //        if (column == null || column.BoardId != boardId)
    //        {
    //            return new Result<CardModel>(ResultCode.NotFound);
    //        }

    //        if (!await _boardStorage.IsUserBoardAsync(userId, boardId))
    //        {
    //            return new Result<CardModel>(ResultCode.Forbidden);
    //        }

    //        if (await _cardStorage.GetCardsCountInColumnAsync(columnId) >= _limitations.MaxCardCount)
    //        {
    //            return new Result<CardModel>(ResultCode.MaxCardCount);
    //        }

    //        var card = new Card
    //        {
    //            Title = title,
    //            Description = "",
    //            OwnerId = userId,
    //            CreatedAt = DateTimeOffset.UtcNow,
    //            ModifiedAt = DateTimeOffset.UtcNow,
    //            ColumnId = columnId,
    //            ModifiedBy = userId
    //        };

    //        card = await _cardStorage.CreateAsync(card);
    //        var cardModel = _mapper.Map<Card, CardModel>(card);

    //        var board = await _boardStorage.GetBoardAsync(boardId);
    //        var eventType = board.OwnerId == userId ?
    //            EventType.CreateCardOnOwnBoard :
    //            EventType.CreateCardOnAlienBoard;

    //        _boardChangesService.DoChange(boardId, userId);

    //        return new Result<CardModel>(cardModel, eventType);
    //    }

    //    [Fact]
    //    public void CreateCard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
    //    {
    //        // Arrange
    //        var mock = new Mock<IColumnStorage>();
    //        mock.Setup(x=>x.GetColumnByIdAsync()).Returns(GetTestUsers());
    //        var controller = new HomeController(mock.Object);

    //        // Act
    //        var result = controller.Index();

    //        // Assert
    //        var viewResult = Assert.IsType<ViewResult>(result);
    //        var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model);
    //        Assert.Equal(GetTestUsers().Count, model.Count());
    //    }
    //}
}
