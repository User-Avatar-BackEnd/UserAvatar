using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserAvatar.DAL.Context;

namespace UserAvatar.DAL
{
    /// <summary>
    /// Migration service
    /// </summary>
    public class MigrationsService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public MigrationsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Starts migration
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            await using var userAvatarContext = scope.ServiceProvider.GetRequiredService<UserAvatarContext>();
            await userAvatarContext.Database.MigrateAsync(cancellationToken);
        }

        /// <summary>
        /// Stops migration
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}