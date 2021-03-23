using Microsoft.Extensions.DependencyInjection;
using System;
using UserAvatar.Api.Options;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Api.Extentions
{
    public static class StorageExtension
    {
        /// <summary>
        /// Registers storages.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddStorages(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            services.AddScoped<IApplicationUser, ApplicationUser>();
            services.AddHttpContextAccessor();

            return services
                .AddTransient<IUserStorage, UserStorage>()
                .AddTransient<IBoardStorage, BoardStorage>()
                .AddTransient<IColumnStorage, ColumnStorage>()
                .AddTransient<IInviteStorage,InviteStorage>()
                .AddTransient<ICommentStorage, CommentStorage>()
                .AddTransient<ICardStorage, CardStorage>();
        }
    }
}
