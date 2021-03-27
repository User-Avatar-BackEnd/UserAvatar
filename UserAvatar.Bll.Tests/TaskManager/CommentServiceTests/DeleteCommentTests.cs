using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.CommentServiceTests
{
    public class DeleteCommentTests
    {
        private readonly Mock<ICommentStorage> _commentStorage;
        private readonly Mock<IBoardStorage> _boardStorage;
        private readonly Mock<ICardStorage> _cardStorage;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IBoardChangesService> _boardChangesService;

        public DeleteCommentTests()
        {
            _commentStorage = new Mock<ICommentStorage>();
            _boardStorage = new Mock<IBoardStorage>();
            _cardStorage = new Mock<ICardStorage>();
            _mapper = new Mock<IMapper>();
            _boardChangesService = new Mock<IBoardChangesService>();
        }

        [Fact]
        public async Task DeleteComment_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var commentService = SetupCommentService();

            // Act
            var result = await commentService.DeleteCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.NotFound, result);
        }

        [Fact]
        public async Task DeleteComment_If_Not_User_Board_Returns_ResultCode_Forbidden()
        {
            // Arrange
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            var commentService = SetupCommentService();

            // Act
            var result = await commentService.DeleteCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.Forbidden, result);
        }

        [Fact]
        public async Task DeleteComment_If_Card_Does_Not_Exist_In_Board_Returns_ResultCode_NotFound()
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
            var result = await commentService.DeleteCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.NotFound, result);
        }

        [Fact]
        public async Task DeleteComment_If_Comment_Does_Not_Exist_In_Card_Returns_ResultCode_NotFound()
        {
            // Arrange
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsBoardCardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _cardStorage.Setup(x => x.IsCardCommentAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            var commentService = SetupCommentService();

            // Act
            var result = await commentService.DeleteCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.NotFound, result);
        }

        [Fact]
        public async Task DeleteComment_If_Successfully_ResultCode_Success()
        {
            // Arrange
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _boardStorage.Setup(x => x.IsBoardCardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _cardStorage.Setup(x => x.IsCardCommentAsync(It.IsAny<int>(), It.IsAny<int>()))
               .ReturnsAsync(true);

            var commentService = SetupCommentService();

            // Act
            var result = await commentService.DeleteCommentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            Assert.Equal(ResultCode.Success, result);
        }


        private CommentService SetupCommentService()
        {
            return new CommentService(
                _commentStorage.Object,
                _boardStorage.Object,
                _cardStorage.Object,
                _mapper.Object,
                _boardChangesService.Object);
        }
    }
}
