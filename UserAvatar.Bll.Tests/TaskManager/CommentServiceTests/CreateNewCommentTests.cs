using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.CommentServiceTests
{
    public class CreateNewCommentTests
    {
        private readonly Mock<ICommentStorage> _commentStorage;
        private readonly Mock<IBoardStorage> _boardStorage;
        private readonly Mock<ICardStorage> _cardStorage;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IBoardChangesService> _boardChangesService;

        public CreateNewCommentTests()
        {
            _commentStorage = new Mock<ICommentStorage>();
            _boardStorage = new Mock<IBoardStorage>();
            _cardStorage = new Mock<ICardStorage>();
            _mapper = new Mock<IMapper>();
            _boardChangesService = new Mock<IBoardChangesService>();
        }

        [Fact]
        public async Task CreateNewComment_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var commentService = SetupCommentService();

            // Act
            var result = await commentService.CreateNewCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            // Assert
            Assert.Equal(ResultCode.NotFound, result.Code);
        }

        [Fact]
        public async Task CreateNewComment_If_Not_User_Board_Returns_ResultCode_Forbidden()
        {
            // Arrange
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            var commentService = SetupCommentService();

            // Act
            var result = await commentService.CreateNewCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            // Assert
            Assert.Equal(ResultCode.Forbidden, result.Code);
        }

        [Fact]
        public async Task CreateNewComment_If_Card_Does_Not_Exist_In_Board_Returns_ResultCode_NotFound()
        {
            // Arrange
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsBoardCardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            var commentService = SetupCommentService();

            // Act
            var result = await commentService.CreateNewCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            // Assert
            Assert.Equal(ResultCode.NotFound, result.Code);
        }

        [Fact]
        public async Task CreateNewComment_If_Successfully_ResultCode_Success()
        {
            // Arrange
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsBoardCardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            var commentService = SetupCommentService();

            // Act
            var result = await commentService.CreateNewCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            // Assert
            Assert.Equal(ResultCode.Success, result.Code);
        }


        private CommentService SetupCommentService()
        {
            return new(
                _commentStorage.Object,
                _boardStorage.Object,
                _cardStorage.Object,
                _mapper.Object,
                _boardChangesService.Object);
        }
    }
}