using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;
using UserAvatar.Bll.Infrastructure;

namespace UserAvatar.Bll.Tests.TaskManager.BoardServiceTests
{
    public class RenameBoardTests
    {
        private readonly Mock<IBoardStorage> _boardStorage;
        private readonly Mock<IMapper> _mapper;
        private readonly IOptions<LimitationOptions> _limitations;

        private readonly Mock<IBoardChangesService> _boardChangesService;

        public RenameBoardTests()
        {
            _limitations = Options.Create(new LimitationOptions());
            _mapper = new Mock<IMapper>();
            _boardStorage = new Mock<IBoardStorage>();
            _boardChangesService = new Mock<IBoardChangesService>();
        }

        [Fact]
        public async Task RenameBoard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
            // Arrange
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync((Board)null);

            var boardService = SetUpService();

            // Act
            var result = await boardService.RenameBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            // Assert
            result.Should().Be(ResultCode.NotFound);
        }

        [Fact]
        public async Task RenameBoard_If_Is_Not_Owner_Returns_ResultCode_Forbidden()
        {
            // Arrange
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board());
            _boardStorage.Setup(x => x.IsOwnerBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            var boardService = SetUpService();

            // Act
            var result = await boardService.RenameBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            // Assert
            result.Should().Be(ResultCode.Forbidden);
        }

        [Fact]
        public async Task RenameBoard_If_Board_Successfully_Remaned_Returns_ResultCode_Success()
        {
            // Arrange
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board());
            _boardStorage.Setup(x => x.IsOwnerBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var boardService = SetUpService();

            // Act
            var result = await boardService.RenameBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

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
