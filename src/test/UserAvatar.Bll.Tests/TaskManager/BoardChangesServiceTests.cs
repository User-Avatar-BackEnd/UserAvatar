using System;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager;

public sealed class BoardChangesServiceTests
{
    [Fact]
    public void HasChanges_If_User_Check_Changes_Time_Bigger_That_LogLifeTime_Returns_True()
    {
        // Arrange
        var logLifeTime = TimeSpan.FromMinutes(1).Ticks;
        var moq = new Mock<IDateTimeProvider>();
        var dateTimeNow = DateTimeOffset.UtcNow.Ticks;
        moq.Setup(x => x.DateTimeUtcNowTicks()).Returns(dateTimeNow);
        var boardChangesService = new BoardChangesService(moq.Object);

        // Act
        var result = boardChangesService.HasChanges(It.IsAny<int>(), It.IsAny<int>(),
            It.Is<long>(x => x < dateTimeNow - logLifeTime));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasChanges_If_Change_Logs_Abscent_Returns_False()
    {
        // Arrange
        var checkTime = TimeSpan.FromSeconds(30).Ticks;
        var moq = new Mock<IDateTimeProvider>();
        var dateTimeNow = DateTimeOffset.UtcNow.Ticks;
        moq.Setup(x => x.DateTimeUtcNowTicks()).Returns(dateTimeNow);

        var boardChangesService = new BoardChangesService(moq.Object);

        // Act
        var result = boardChangesService.HasChanges(It.IsAny<int>(), It.IsAny<int>(), dateTimeNow - checkTime);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasChanges_If_Only_This_User_Change_Board_Returns_False()
    {
        // Arrange
        var boardId = 1;
        var userId = 1;

        var checkTime = TimeSpan.FromSeconds(30).Ticks;
        var changeTime = TimeSpan.FromSeconds(15).Ticks;
        var dateTimeNow = DateTimeOffset.UtcNow.Ticks;

        var moq = new Mock<IDateTimeProvider>();
        moq.Setup(x => x.DateTimeUtcNowTicks()).Returns(dateTimeNow);

        var boardChangesService = new BoardChangesService(moq.Object);

        // Act
        boardChangesService.DoChange(boardId, userId);
        var result = boardChangesService.HasChanges(boardId, userId, dateTimeNow - checkTime);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasChanges_If_Another_User_Change_Board_Returns_True()
    {
        // Arrange
        var boardId = 1;
        var userId = 1;

        var checkTime = TimeSpan.FromSeconds(30).Ticks;
        var changeTime = TimeSpan.FromSeconds(15).Ticks;
        var dateTimeNow = DateTimeOffset.UtcNow.Ticks;

        var moq = new Mock<IDateTimeProvider>();
        moq.Setup(x => x.DateTimeUtcNowTicks()).Returns(dateTimeNow);

        var boardChangesService = new BoardChangesService(moq.Object);

        // Act
        boardChangesService.DoChange(boardId, It.Is<int>(x => x != userId));
        var result = boardChangesService.HasChanges(boardId, userId, dateTimeNow - checkTime);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasChanges_If_Another_User_Change_Another_Board_Returns_False()
    {
        // Arrange
        var boardId = 1;
        var userId = 1;

        var checkTime = TimeSpan.FromSeconds(30).Ticks;
        var changeTime = TimeSpan.FromSeconds(15).Ticks;
        var dateTimeNow = DateTimeOffset.UtcNow.Ticks;

        var moq = new Mock<IDateTimeProvider>();
        moq.Setup(x => x.DateTimeUtcNowTicks()).Returns(dateTimeNow);

        var boardChangesService = new BoardChangesService(moq.Object);

        // Act
        boardChangesService.DoChange(It.Is<int>(x => x != boardId), It.Is<int>(x => x != userId));
        var result = boardChangesService.HasChanges(boardId, userId, dateTimeNow - checkTime);

        // Assert
        Assert.False(result);
    }
}
