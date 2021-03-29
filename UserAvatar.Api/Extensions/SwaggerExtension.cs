using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace UserAvatar.Api.Extensions
{
    /// <summary>
    /// Swagger extension
    /// </summary>
    public static class SwaggerExtension
    {
        /// <summary>
        /// Registers swagger.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            
            services.AddSwaggerGen(options =>
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "UserAvatar", Version = "v1" });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                },
                            },
                            new string[0]
                        }
                    });

                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Name = "Authorization",
                        Description = "JWT token",
                        BearerFormat = "JWT"
                    });
            });

            return services;
        }
    }
}