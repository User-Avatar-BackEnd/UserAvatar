using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserAvatar.Api.Options;

namespace UserAvatar.Api.Extensions;

/// <summary>
///     Authentication services extension
/// </summary>
public static class AuthenticationExtension
{
    /// <summary>
    ///     Register authentication
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns></returns>
    public static IServiceCollection AddAuthentications(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

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

        return services;
    }
}
