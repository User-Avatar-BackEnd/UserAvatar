using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Gamification.Services;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.Gamification;

public sealed class EventServiceTests
{
    private readonly Mock<IEventStorage> _eventStorage;
    private readonly Mock<IHistoryService> _historyService;
    private readonly IMapper _mapper;
    private readonly Mock<IUserStorage> _userStorage;

    public EventServiceTests()
    {
        _eventStorage = new Mock<IEventStorage>();
        _historyService = new Mock<IHistoryService>();
        _userStorage = new Mock<IUserStorage>();

        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            _mapper = mapper;
        }
    }

    [Fact]
    public async Task GetEventList_Returns_EventList()
    {
        // Arrange
        _eventStorage.Setup(x => x.GetEventListAsync()).ReturnsAsync(new List<Event>
        {
            new() { Name = "Registration", Score = 20 },
            new() { Name = "Login", Score = 15 },
            new() { Name = "Logout", Score = 13 },
            new() { Name = "Create board", Score = 10 }
        });

        var newEvent = new List<EventModel>
        {
            new() { Name = "Registration", Score = 20 },
            new() { Name = "Login", Score = 15 },
            new() { Name = "Logout", Score = 13 },
            new() { Name = "Create board", Score = 10 }
        };

        var eventService = SetUpService();

        // Act
        var result = await eventService.GetEventListAsync();

        // Assert
        IsEquals(result, newEvent).Should().BeTrue();
    }

    [Fact]
    public async Task ChangeEventsCost_If_Invalid_Event_List_Returns_ResultCode_BadRequest()
    {
        // Assert
        _eventStorage.Setup(x => x.GetEventListAsync()).ReturnsAsync(new List<Event>
        {
            new() { Name = "Registration", Score = 20 },
            new() { Name = "Login", Score = 15 },
            new() { Name = "Logout", Score = 13 },
            new() { Name = "Create board", Score = 10 }
        });

        var newEvent1 = new List<EventModel>
        {
            new() { Name = "Registration", Score = 40 }, new() { Name = "Login", Score = 5 }
        };

        var newEvent2 = new List<EventModel>
        {
            new() { Name = "Commenting", Score = 20 },
            new() { Name = "Looking", Score = 15 },
            new() { Name = "Sending emodji", Score = 13 },
            new() { Name = "Create board", Score = 10 }
        };

        var eventService = SetUpService();

        // Act
        var result1 = await eventService.ChangeEventsCostAsync(newEvent1);
        var result2 = await eventService.ChangeEventsCostAsync(newEvent2);

        // Assert
        result1.Should().Be(ResultCode.BadRequest);
        result2.Should().Be(ResultCode.BadRequest);
    }

    [Fact]
    public async Task ChangeEventsCost_If_Valid_Event_List_Returns_ResultCode_Success()
    {
        // Assert
        _eventStorage.Setup(x => x.GetEventListAsync()).ReturnsAsync(new List<Event>
        {
            new() { Name = "Registration", Score = 20 },
            new() { Name = "Login", Score = 15 },
            new() { Name = "Logout", Score = 13 },
            new() { Name = "Create board", Score = 10 }
        });

        var newEvent = new List<EventModel>
        {
            new() { Name = "Registration", Score = 20 },
            new() { Name = "Login", Score = 15 },
            new() { Name = "Logout", Score = 13 },
            new() { Name = "Create board", Score = 10 }
        };

        var eventService = SetUpService();

        // Act
        var result = await eventService.ChangeEventsCostAsync(newEvent);

        // Assert
        result.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task ChangeBalance_If_User_Does_Not_Exist_Returns_ResultCode_NotFound()
    {
        // Arrange
        _userStorage.Setup(x => x.GetByLoginAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        var eventService = SetUpService();

        // Act
        var result = await eventService.ChangeBalanceAsync(It.IsAny<string>(), It.IsAny<int>());

        //Assert
        result.Should().Be(ResultCode.NotFound);
    }

    [Fact]
    public async Task ChangeBalance_If_User_Exists_Returns_ResultCode_Succcess()
    {
        // Arrange
        _userStorage.Setup(x => x.GetByLoginAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var eventService = SetUpService();

        // Act
        var result = await eventService.ChangeBalanceAsync(It.IsAny<string>(), It.IsAny<int>());

        //Assert
        result.Should().Be(ResultCode.Success);
    }

    private EventService SetUpService()
    {
        return new EventService(
            _eventStorage.Object,
            _historyService.Object,
            _mapper,
            _userStorage.Object);
    }

    private bool IsEquals(List<EventModel> events1, List<EventModel> events2)
    {
        for (var i = 0; i < events1.Count(); i++)
        {
            if (events1[i].Name != events2[i].Name ||
                events1[i].Score != events2[i].Score)
            {
                return false;
            }
        }

        return true;
    }
}
