using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.BoardServiceTests
{
    public class DeleteMemberFromBoardTests
    {
        private readonly Mock<IBoardStorage> _boardStorage;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IBoardChangesService> _boardChangesService;
        private readonly IOptions<LimitationOptions> _limitations;

        public DeleteMemberFromBoardTests()
        {
            _limitations = Options.Create(new LimitationOptions());
            _mapper = new Mock<IMapper>();
            _boardStorage = new Mock<IBoardStorage>();
            _boardChangesService = new Mock<IBoardChangesService>();
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_User_Equals_ToDeleteUser_Returns_ResultCode_Forbidden()
        {
            // Arrange
            var userId = 1;

            var boardService = SetUpService();

            // Act
            var result = await boardService.DeleteMemberFromBoardAsync(userId, userId, It.IsAny<int>());

            //Assert
            result.Should().Be(ResultCode.Forbidden);
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_Board_Does_Not_Exit_Returns_ResultCode_NotFound()
        {
            // Arrange
            var userId = 1;

            _boardStorage.Setup(method => method.GetBoardAsync(It.IsAny<int>())).ReturnsAsync((Board)null);

            var boardService = SetUpService();

            // Act
            var result = await boardService.DeleteMemberFromBoardAsync(userId, It.Is<int>(toDeleteUserId => toDeleteUserId != userId), It.IsAny<int>());

            // Assert
            result.Should().Be(ResultCode.NotFound);
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_User_Is_Not_Owner_Returns_ResultCode_Forbidden()
        {
            // Arrange
            var userId = 1;

            _boardStorage.Setup(method => method.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board { OwnerId = It.Is<int>(ownerId => ownerId != userId) });

            var boardService = SetUpService();

            // Act
            var result = await boardService.DeleteMemberFromBoardAsync(userId, It.Is<int>(toDeleteUserId => toDeleteUserId != userId), It.IsAny<int>());

            // Assert
            result.Should().Be(ResultCode.Forbidden);
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_User_Is_Not_Board_Member_Returns_ResultCode_NotFound()
        {
            // Arrange
            var userId = 1;
            var deletedId = 2;

            _boardStorage.Setup(method => method.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board
            {
                OwnerId = userId,
                Members = new List<Member>(){
                new Member { UserId = It.Is<int>(x=> x != deletedId)}
                }
            });

            var boardService = SetUpService();

            // Act
            var result = await boardService.DeleteMemberFromBoardAsync(userId, deletedId, It.IsAny<int>());

            // Assert
            result.Should().Be(ResultCode.NotFound);
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_User_Is_Board_Member_Returns_ResultCode_Success()
        {
            // Arrange
            var userId = 1;
            var deletedId = 2;
            var boardId = 3;

            _boardStorage.Setup(method => method.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board
            {
                OwnerId = userId,
                Members = new List<Member>(){
                new Member { UserId = deletedId}
                }
            });

            _boardStorage.Setup(x => x.GetMemberByIdAsync(deletedId, boardId)).ReturnsAsync(new Member());

            var boardService = SetUpService();

            // Act
            var result = await boardService.DeleteMemberFromBoardAsync(userId, deletedId, boardId);

            // Assert
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
}
