using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace UserAvatar.Bll.Tests.TaskManager;

public sealed class ColumnServiceTests
{
    private readonly Mock<IBoardChangesService> _boardChangesService;
    private readonly Mock<IBoardStorage> _boardStorage;
    private readonly Mock<IColumnStorage> _columnStorage;
    private readonly IOptions<LimitationOptions> _limitations;
    private readonly Mapper _mapper;
    private readonly ITestOutputHelper _testOutputHelper;
    private ColumnService _service;

    public ColumnServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _columnStorage = new Mock<IColumnStorage>();
        _boardStorage = new Mock<IBoardStorage>();
        _boardChangesService = new Mock<IBoardChangesService>();
        var myProfile = new MappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        _mapper = new Mapper(configuration);
        _limitations = Options.Create(new LimitationOptions { MaxColumnCount = 10 });
    }

    private ColumnService SetupColumnService()
    {
        return new ColumnService(
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
        _boardStorage.Setup(x => x.IsBoardExistAsync(It.Is<int>(boardId => boardId != 1)))
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
        _boardStorage.Setup(x => x.IsUserBoardAsync(It.Is<int>(value => value == 1), It.IsAny<int>()))
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
            .ReturnsAsync(columnCount - 1);

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
    public async Task GetColumnByIdAsync_If_Column_is_Null_Returns_NotFound()
    {
        //Arrange
        SetupAnyDeps();
        _boardStorage.Setup(x => x.IsBoardColumnAsync(It.IsAny<int>(), It.Is<int>(columnId => columnId > 10)))
            .ReturnsAsync(false);
        _service = SetupColumnService();

        //Act
        var resultError =
            await _service.GetColumnByIdAsync(It.IsAny<int>(), It.IsAny<int>(), 11);
        var resultSuccess =
            await _service.GetColumnByIdAsync(It.IsAny<int>(), It.IsAny<int>(), 9);

        //Assert
        resultError.Code.Should().Be(ResultCode.NotFound);
        resultSuccess.Code.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task DeleteAsync_If_Column_is_Deleted_Returns_BadRequest()
    {
        //Arrange
        SetupAnyDeps();
        _columnStorage.Setup(x => x.InternalGetAllColumnsAsync(It.IsAny<Column>()))
            .ReturnsAsync(new List<Column> { new() { Index = 0 }, new() { Index = 1 }, new() { Index = 2 } });

        _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>())).ReturnsAsync(new Column { IsDeleted = true });
        _service = SetupColumnService();

        //Act
        var resultError =
            await _service.DeleteAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        //Assert
        resultError.Should().Be(ResultCode.BadRequest);
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
            await _service.UpdateAsync(It.IsAny<int>(), It.IsAny<int>(), 11, It.IsAny<string>());
        var resultSuccess =
            await _service.UpdateAsync(It.IsAny<int>(), It.IsAny<int>(), 9, It.IsAny<string>());

        //Assert
        resultError.Should().Be(ResultCode.NotFound);
        resultSuccess.Should().Be(ResultCode.Success);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(4)]
    public async Task CreateAsync_Returns_New_Column(int columnCount)
    {
        //Arrange
        SetupAnyDeps();
        var expected = new Column
        {
            Title = It.IsAny<string>(), BoardId = columnCount, CreatedAt = default, ModifiedAt = default
        };
        _columnStorage.Setup(x => x.GetColumnsCountInBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(columnCount);
        /*_columnStorage.Setup(x => x.AddColumnAsync(It.Is<Column>(boardId=> boardId.BoardId == columnCount)))
            .ReturnsAsync(expected);*/

        _service = SetupColumnService();

        //Act
        var result =
            await _service.CreateAsync(It.IsAny<int>(), columnCount, It.IsAny<string>());


        //Assert
        Assert.Equal(expected.BoardId, result.Value.BoardId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(148)]
    public async Task GetColumnById_If_Id_Exists_Returns_Column(int columnId)
    {
        //Arrange
        SetupAnyDeps();
        var expected = new ColumnModel { Id = columnId };
        _columnStorage.Setup(x => x.GetColumnByIdAsync(columnId))
            .ReturnsAsync(new Column { Id = columnId });
        _service = SetupColumnService();

        //Act
        var result = await _service.GetColumnByIdAsync(It.IsAny<int>(), It.IsAny<int>(), columnId);


        //Assert
        Assert.StrictEqual(expected.Id, result.Value.Id);
    }

    [Fact]
    public async Task UpdateAsync_If_Updated_Returns_Success()
    {
        //Arrange
        SetupAnyDeps();
        _columnStorage.Setup(x => x.GetColumnByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Column());
        _service = SetupColumnService();

        //Act
        var result = await _service.UpdateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

        //Assert
        result.Should().Be(ResultCode.Success);
    }

    [Theory]
    [InlineData(1, 0, 2)]
    [InlineData(2, 1, 0)]
    [InlineData(3, 2, 1)]
    public async Task ChangePositionAsync_Returns_New_Column_Position(int columnId, int oldIndex, int newIndex)
    {
        //Arrange
        SetupAnyDeps();

        _columnStorage.Setup(x => x.GetColumnByIdAsync(columnId))
            .ReturnsAsync(new Column { Index = oldIndex });

        _columnStorage.Setup(x => x.GetColumnsCountInBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(3);

        _columnStorage.Setup(x => x.GetAllColumnsExceptThisAsync(It.IsAny<Column>()))
            .ReturnsAsync(new List<Column> { new() { Index = 0 }, new() { Index = 1 }, new() { Index = 2 } });

        _service = SetupColumnService();

        //Act
        var algorithm = await _service
            .ChangePositionAsync(It.IsAny<int>(), It.IsAny<int>(), columnId, newIndex);

        //Assert
        Assert.Equal(ResultCode.Success, algorithm);
    }
}
