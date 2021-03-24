using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.Dal.Context;

namespace UserAvatar.Api.Extentions
{
    public static class DbContextExtension
    {
    /// <summary>
    /// Register Db contexts
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns></returns>
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDbContext<UserAvatarContext>(
                   options =>
                       options.UseNpgsql(
                           configuration.GetConnectionString("connectionString"),
                           x => x.MigrationsAssembly("UserAvatar.Dal")));

            return services;
        }
    }
}
