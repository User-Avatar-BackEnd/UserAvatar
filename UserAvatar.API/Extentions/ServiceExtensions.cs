using System;
using Microsoft.Extensions.DependencyInjection;
using UserAvatar.BLL.Services;
using UserAvatar.BLL.Services.Interfaces;
using UserAvatar.DAL.Storages;
using UserAvatar.DAL.Storages.Interfaces;

namespace AuthWebApps.AuthServices.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers services.
        /// </summary>
        /// <typeparam name="TSessionData">Session data type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddServices<TSessionData>(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            return services
                .AddTransient<IUserStorage, UserStorage>()
                .AddTransient<IBoardStorage, BoardStorage>()
                .AddTransient<IColumnStorage, ColumnStorage>()
                .AddTransient<IAuthService, AuthService>()
                .AddTransient<IBoardService, BoardService>()
                .AddTransient<IColumnService, ColumnService>();
            //services.AddTransient<ITaskService, TaskService>();
        }
    }
}
