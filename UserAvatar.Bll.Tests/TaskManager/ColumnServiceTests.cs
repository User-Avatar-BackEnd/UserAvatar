using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Storages.Interfaces;
using FluentAssertions;
using UserAvatar.Api.Extentions;
using UserAvatar.Dal.Entities;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager
{
    public class ColumnServiceTests
    {
        private readonly Mapper _mapper;
        private readonly Mock<IColumnStorage> _columnStorage;
        private readonly Mock<IBoardStorage> _boardStorage;
        private readonly Mock<IBoardChangesService> _boardChangesService;
        private readonly IOptions<LimitationOptions> _limitations;
        private ColumnService _service;
        public ColumnServiceTests()
        {
            _columnStorage = new Mock<IColumnStorage>();
            _boardStorage = new Mock<IBoardStorage>();
            _boardChangesService = new Mock<IBoardChangesService>();
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            _mapper = new Mapper(configuration);
            _limitations = Options.Create(new LimitationOptions
            {
                MaxColumnCount = 10
            });
        }
        private ColumnService SetupColumnService()
        {
            return new(
                _columnStorage.Object, 
                _mapper, 
                _boardStorage.Object,
                _limitations,
                _boardChangesService.Object);
        }

        private void SetupAnyDeps()
        {
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
        }
        
        [Fact]
        public async Task CreateColumn_If_Board_Not_Exists_Returns_NotFound()
        {
            //Assert
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.Is<int>(boardId=> boardId!=1)))
                .ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _service = SetupColumnService();
            
            //Act
            var resultedError = await _service.CreateAsync(It.IsAny<int>(), 1, It.IsAny<string>());
            var resultedSuccess = await _service.CreateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Arrange
            resultedError.Code.Should().Be(ResultCode.NotFound);
            resultedSuccess.Code.Should().Be(ResultCode.Success);
        }
        
        [Fact]
        public async Task CreateColumn_If_User_Is_Member_Returns_Forbidden()
        {
            //Assert
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.Is<int>(value=> value==1), It.IsAny<int>()))
                .ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _service = SetupColumnService();

            //Act
            var resultedError = await _service.CreateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            var resultedSuccess = await _service.CreateAsync(1, It.IsAny<int>(), It.IsAny<string>());

            //Arrange
            resultedError.Code.Should().Be(ResultCode.Forbidden);
            resultedSuccess.Code.Should().Be(ResultCode.Success);
        }
        
        [Fact]
        public async Task CreateColumn_If_Maximal_Limit_Returns_MaxColumnCount()
        {
            //Assert
            SetupAnyDeps();
            var columnCount = _limitations.Value.MaxColumnCount;
            _service = SetupColumnService();
            _columnStorage.Setup(x =>
                    x.GetColumnsCountInBoardAsync(It.IsAny<int>()))
                .ReturnsAsync(columnCount);
            
            //Act
            var resultedError = await _service.CreateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            
            //Arrange
            resultedError.Code.Should().Be(ResultCode.MaxColumnCount);
            
            //Assert
            _columnStorage.Setup(x =>
                    x.GetColumnsCountInBoardAsync(It.IsAny<int>()))
                .ReturnsAsync(columnCount-1);
            
            //Act
            var resultedSuccess = await _service.CreateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Arrange
            resultedSuccess.Code.Should().Be(ResultCode.Success);
        }
        
        [Fact]
        public async Task ChangePositionAsync_If_Less_Than_Position_Returns_Bad_Request()
        {
            //Assert
            const int positionIndex = 5;
            SetupAnyDeps();
            _columnStorage.Setup(x => x.GetColumnsCountInBoardAsync(It.IsAny<int>()))
                .ReturnsAsync(positionIndex - 1);
            _service = SetupColumnService();
            
            //Act
            var result =
                await _service.ChangePositionAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), positionIndex);
            
            //Arrange
            result.Should().Be(ResultCode.BadRequest);
            
        }
        
        [Fact]
        public async Task ChangePositionAsync_If_More_Than_Position_Returns_Ok()
        {
            //Assert
            const int positionIndex = 5;
            SetupAnyDeps();
            _columnStorage.Setup(x => x.GetColumnsCountInBoardAsync(It.IsAny<int>()))
                .ReturnsAsync(positionIndex + 1);
            
            _service = SetupColumnService();
            
            //Act
            var result =
                await _service.ChangePositionAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), positionIndex);
            
            //Assert
            result.Should().Be(ResultCode.Success);
        }
        
        [Fact]
        public async Task GetColumnByIdAsync_If_Column_is_Null_Returns_NotFound()
        {
            //Arrange
            SetupAnyDeps();
            _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.Is<int>(columnId => columnId > 10)))
                .ReturnsAsync(false);
            _service = SetupColumnService();
            
            //Act
            var resultError =
                await _service.GetColumnByIdAsync(It.IsAny<int>(), It.IsAny<int>(),11);
            var resultSuccess =
                await _service.GetColumnByIdAsync(It.IsAny<int>(), It.IsAny<int>(),9);
            
            //Assert
            resultError.Code.Should().Be(ResultCode.NotFound);
            resultSuccess.Code.Should().Be(ResultCode.Success);

        }
        [Fact]
        public async Task DeleteAsync_If_Column_is_Null_Returns_NotFound()
        {
            //Arrange
            SetupAnyDeps();
            _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.Is<int>(columnId => columnId > 10)))
                .ReturnsAsync(false);
            _service = SetupColumnService();
            
            //Act
            var resultError =
                await _service.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(),11);
            var resultSuccess =
                await _service.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(),9);
            
            //Assert
            resultError.Should().Be(ResultCode.NotFound);
            resultSuccess.Should().Be(ResultCode.Success);

        }
        [Fact]
        public async Task UpdateAsync_If_Column_is_Null_Returns_NotFound()
        {
            //Arrange
            SetupAnyDeps();
            _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.Is<int>(columnId => columnId > 10)))
                .ReturnsAsync(false);
            _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>())).ReturnsAsync(new Column());
            _service = SetupColumnService();
            
            //Act
            var resultError =
                await _service.UpdateAsync(It.IsAny<int>(), It.IsAny<int>(),11, It.IsAny<string>());
            var resultSuccess =
                await _service.UpdateAsync(It.IsAny<int>(), It.IsAny<int>(),9,It.IsAny<string>());
            
            //Assert
            resultError.Should().Be(ResultCode.NotFound);
            resultSuccess.Should().Be(ResultCode.Success);

        }
    }
}