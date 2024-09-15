using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserAvatar.Dal.Context;

namespace UserAvatar.Api.Extensions;

/// <summary>
///     Database context service extension
/// </summary>
public static class DbContextExtension
{
    /// <summary>
    ///     Adds Data base context
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddDbContext<UserAvatarContext>(
            options => options.UseNpgsql(
                configuration.GetConnectionString("connectionString"),
                x => x.MigrationsAssembly("UserAvatar.Dal")));

        return services;
    }
}
