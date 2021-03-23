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
            
            services.AddOptions<JwtOptions>();
            var jwtOptions = services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<JwtOptions>>()
                .Value;
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = jwtOptions.RequireHttps;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddScoped<IApplicationUser, ApplicationUser>();
            services.AddHttpContextAccessor();
            
            return services
                .AddTransient<IUserStorage, UserStorage>()
                .AddTransient<IBoardStorage, BoardStorage>()
                .AddTransient<IColumnStorage, ColumnStorage>()
                .AddTransient<ICommentStorage,CommentStorage>()
                .AddTransient<ICardStorage,CardStorage>()
                .AddTransient<IAuthService, AuthService>()
                .AddTransient<IBoardService, BoardService>()
                .AddTransient<IColumnService, ColumnService>()
                .AddTransient<ICardService, CardService>()
                .AddTransient<ICommentService,CommentService>()
                .AddTransient<IPersonalAccountService,PersonalAccountService>();
        }
    }
}
