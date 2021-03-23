using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserAvatar.Api.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Api.Extentions
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

            services.AddAutoMapper(c => c.AddProfile<MappingProfile>(), typeof(Startup));

            return services
                .AddTransient<IAuthService, AuthService>()
                .AddTransient<IBoardService, BoardService>()
                .AddTransient<IColumnService, ColumnService>()
                .AddTransient<ICardService, CardService>()
                .AddTransient<ICommentService,CommentService>()
                .AddTransient<IPersonalAccountService,PersonalAccountService>();
        }
    }
}
