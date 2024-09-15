using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.Gamification.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.Gamification;

public sealed class RankServiceTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRankStorage> _rankStorage;

    public RankServiceTests()
    {
        _rankStorage = new Mock<IRankStorage>();

        var myProfile = new MappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        _mapper = new Mapper(configuration);
    }

    [Theory]
    [InlineData(0, 0, 100)]
    [InlineData(0, 20, 100)]
    [InlineData(300, 400, 500)]
    [InlineData(900, 920, 1000)]
    [InlineData(1000, 1200, int.MaxValue)]
    public async Task GetAllRanksData_Returns_Correct_Model(int min, int current, int max)
    {
        // Arrange
        _rankStorage.Setup(x => x.GetAllRankAsync()).ReturnsAsync(GetDefaultRanks());

        var rankService = new RankService(_rankStorage.Object, _mapper);

        // Act
        var result = await rankService.GetAllRanksDataAsync(current);

        // Assert
        Assert.Equal(min, result.Score);
        Assert.Equal(max, result.MaxScores);
    }

    [Fact]
    public async Task GetRanks_Returns_Correct_Ranks()
    {
        // Arrange
        var inputData = new List<int>
        {
            0,
            10,
            100,
            300,
            500,
            700,
            900,
            999,
            1000,
            1300
        };
        var outputData = new List<string>
        {
            "NPC",
            "NPC",
            "Crewman",
            "Cossack",
            "Centurion",
            "Esaul",
            "Ataman",
            "Ataman",
            "Hetman",
            "Hetman"
        };

        _rankStorage.Setup(x => x.GetAllRankAsync()).ReturnsAsync(GetDefaultRanks());

        var rankService = new RankService(_rankStorage.Object, _mapper);

        // Act
        var result = await rankService.GetRanksAsync(inputData);

        // Assert
        Assert.Equal(outputData, result);
    }

    private List<Rank> GetDefaultRanks()
    {
        return new List<Rank>
        {
            new() { Name = "NPC", Score = 0 },
            new() { Name = "Crewman", Score = 100 },
            new() { Name = "Cossack", Score = 300 },
            new() { Name = "Centurion", Score = 500 },
            new() { Name = "Esaul", Score = 700 },
            new() { Name = "Ataman", Score = 900 },
            new() { Name = "Hetman", Score = 1000 }
        };
    }
}
