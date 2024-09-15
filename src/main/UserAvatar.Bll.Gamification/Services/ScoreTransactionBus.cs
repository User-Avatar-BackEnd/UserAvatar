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
public sealed class ScoreTransactionBus : IHostedService
{
    private readonly ILogger<ScoreTransactionBus> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer _timer;


    public ScoreTransactionBus(IServiceScopeFactory scopeFactory,
        ILogger<ScoreTransactionBus> logger)
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
        //var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserAvatarContext>();
        var period = TimeSpan.FromSeconds(10);
        // reset cache every midnight (UTC)
        _timer = new Timer(MakeTransactions, _scopeFactory, TimeSpan.FromSeconds(10), period);

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

    private void MakeTransactions(object state)
    {
        _logger.LogInformation("Starting seeking not calculated transactions");
        var factory = (IServiceScopeFactory)state;
        using var scope = factory.CreateScope();
        var rates = scope.ServiceProvider.GetRequiredService<IHistoryService>();
        rates.MakeScoreTransactionAsync().GetAwaiter().GetResult();
        _logger.LogInformation("Seeking done. Calculated");
    }
}
