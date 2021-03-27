using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
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

    public class GetBoardTests
    {
        private readonly Mock<IBoardStorage> _boardStorage;
        private readonly Mock<IMapper> _mapper;
        private readonly IOptions<LimitationOptions> _limitations;

        private readonly Mock<IBoardChangesService> _boardChangesService;

        public GetBoardTests()
        {
            _limitations = Options.Create(new LimitationOptions());
            _mapper = new Mock<IMapper>();
            _boardStorage = new Mock<IBoardStorage>();
            _boardChangesService = new Mock<IBoardChangesService>();
        }

        [Fact]
        public async Task GetBoard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync((Board)null);

            var boardService = SetUpService();

            // Act
            var result = await boardService.GetBoardAsync(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            result.Code.Should().Be(ResultCode.NotFound);
        }

        [Fact]
        public async Task GetBoard_If_User_Does_Not_Relate_To_Board_Returns_ResultCode_Forbidden()
        {
            // Arrange
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board());
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            var boardService = SetUpService();

            // Act
            var result = await boardService.GetBoardAsync(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            result.Code.Should().Be(ResultCode.Forbidden);
        }

        [Fact]
        public async Task GetBoard_If_User_Relate_To_Board_Returns_ResultCode_Success()
        {
            // Arrange
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board());
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var boardService = SetUpService();

            // Act
            var result = await boardService.GetBoardAsync(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            result.Code.Should().Be(ResultCode.Success);
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
