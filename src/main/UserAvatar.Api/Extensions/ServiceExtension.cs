﻿using System;
using Microsoft.Extensions.DependencyInjection;
using UserAvatar.Api.Authentication;
using UserAvatar.Bll.Gamification.Services;
using UserAvatar.Bll.Gamification.Services.Interfaces;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;

namespace UserAvatar.Api.Extensions;

/// <summary>
///     All services injection extension
/// </summary>
public static class ServiceExtension
{
    /// <summary>
    ///     Registers services.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddAutoMapper(c => c.AddProfile<MappingProfile>(), typeof(Startup));

        services
            .AddTransient<IAuthService, AuthService>()
            .AddTransient<IBoardService, BoardService>()
            .AddTransient<IColumnService, ColumnService>()
            .AddTransient<ICardService, CardService>()
            .AddTransient<IInviteService, InviteService>()
            .AddTransient<ICommentService, CommentService>()
            .AddTransient<IPersonalAccountService, PersonalAccountService>()
            .AddTransient<IEventService, EventService>()
            .AddTransient<IHistoryService, HistoryService>()
            .AddTransient<IRankService, RankService>()
            .AddTransient<IRateService, RateService>()
            .AddSingleton<IBoardChangesService, BoardChangesService>()
            .AddTransient<ISearchService, SearchService>()
            .AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IApplicationUser, ApplicationUser>();

        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        return services;
    }
}
