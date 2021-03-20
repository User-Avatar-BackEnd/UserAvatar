using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserAvatar.API.Options;
using UserAvatar.BLL.Services;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Storages;

namespace UserAvatar.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!Environment.IsDevelopment())
            {
                services.AddDbContext<UserAvatarContext>(
                    options =>
                        options.UseSqlite(
                            Configuration.GetConnectionString("sqliteConn"),
                            x => x.MigrationsAssembly("UserAvatar.DAL")), ServiceLifetime.Transient);
            }
            else
            {
                services.AddDbContext<UserAvatarContext>(
                    options =>
                        options.UseNpgsql(
                            Configuration.GetConnectionString("connectionString"),
                            x => x.MigrationsAssembly("UserAvatar.DAL")), ServiceLifetime.Transient);
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


            services.AddTransient<UserStorage>();
            services.AddTransient<BoardStorage>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IBoardService, BoardService>();
            
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
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
        }
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserAvatar v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
