using System;
using Microsoft.Extensions.DependencyInjection;
using UserAvatar.BLL.Services;
using UserAvatar.BLL.Services.Interfaces;
using UserAvatar.DAL.Storages;
using UserAvatar.DAL.Storages.Interfaces;

namespace UserAvatar.API.Extentions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers services.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            return services
                .AddTransient<IUserStorage, UserStorage>()
                .AddTransient<IBoardStorage, BoardStorage>()
                .AddTransient<IColumnStorage, ColumnStorage>()
                .AddTransient<ICardStorage,CardStorage>()
                .AddTransient<IAuthService, AuthService>()
                .AddTransient<IBoardService, BoardService>()
                .AddTransient<IColumnService, ColumnService>()
                .AddTransient<ICardService, CardService>();
        }
    }
}
