using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Api.Extentions;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager
{
    public class CardServiceTests
    {
        private readonly Mock<ICardStorage> _cardStorage;
        private readonly Mock<IBoardStorage> _boardStorage;
        private readonly Mock<IColumnStorage> _columnStorage;
        private readonly IOptions<LimitationOptions> _limitations;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IBoardChangesService> _boardChangesService;

        public CardServiceTests()
        {
            _mapper = new Mock<IMapper>();
            _columnStorage = new Mock<IColumnStorage>();
            _boardStorage = new Mock<IBoardStorage>();
            _boardChangesService = new Mock<IBoardChangesService>();
            _cardStorage = new Mock<ICardStorage>();

            _limitations = Options.Create(new LimitationOptions
            {
                MaxCardCount = 100
            });
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

        [Fact]
        public async Task CreateCard_If_Column_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Column)null);

            var cardService = SetupCardService();

            // Act
            var result = await cardService.CreateCardAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

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

            _cardStorage.Setup(x=>x.GetByIdAsync(It.IsAny<int>()))
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
                new CardModel { ColumnId = It.Is<int>(x => x != oldColumnId) },
                It.IsAny<int>(),
                It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.Success, result.Code);
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
                new CardModel { ColumnId = oldColumnId },
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
                new CardModel { ColumnId = oldColumnId },
                It.IsAny<int>(),
                userId);

            // Assert
            Assert.Equal(ResultCode.Success, result.Code);
            Assert.Equal(EventType.ChangeCardStatusOnAlienBoard, result.EventType);
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



        [Fact]
        public async Task GetById_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Card)null);

            var cardService = SetupCardService();

            // Act
            var result = await cardService.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.NotFound, result.Code);
        }

        [Fact]
        public async Task GetById_If_Not_User_Board_Returns_ResultCode_NotFound()
        {
            // Arrange
            _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Card());

            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            var cardService = SetupCardService();

            // Act
            var result = await cardService.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.Forbidden, result.Code);
        }

        [Fact]
        public async Task GetById_If_Success_Deleted_Returns_ResultCode_Success()
        {
            // Arrange
            _cardStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Card());

            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            var cardService = SetupCardService();

            // Act
            var result = await cardService.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.Success, result.Code);
        }
    }
}