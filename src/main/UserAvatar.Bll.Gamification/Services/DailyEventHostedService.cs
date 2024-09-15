using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserAvatar.Bll.Gamification.Services.Interfaces;

namespace UserAvatar.Bll.Gamification.Services;

/// <summary>
///     Service for accounting user score by events
/// </summary>
public sealed class DailyQuestsHostedService : IHostedService
{
    private readonly ILogger<DailyQuestsHostedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer _timer;

    public DailyQuestsHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<DailyQuestsHostedService> logger)
    {
        _scopeFactory = scopeFactory
                        ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger;
    }

    /// <summary>
    ///     Start Timer to actualize
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // reset cache every midnight (UTC)
        _timer = new Timer(GenerateDailyQuests, _scopeFactory, TimeSpan.FromSeconds(10), TimeSpan.FromDays(1d));

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Stops actualizing data
    /// </summary>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    private void GenerateDailyQuests(object state)
    {
        _logger.LogInformation("Starting Generating daily quests");
        var factory = (IServiceScopeFactory)state;
        using var scope = factory.CreateScope();
        var rates = scope.ServiceProvider.GetRequiredService<IEventService>();
        rates.GenerateDailyQuests().GetAwaiter().GetResult();
        _logger.LogInformation("Daily Quests successfully generated!");
    }
}
