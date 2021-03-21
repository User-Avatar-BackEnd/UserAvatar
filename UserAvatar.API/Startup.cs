using System;
using System.Linq;
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
using UserAvatar.Api.Options;
using UserAvatar.Bll.Services;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using AutoMapper;
using AuthWebApps.AuthServices.Extensions;
using UserAvatar.Api.Extentions;

namespace UserAvatar.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            var mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);

            if (Environment.IsDevelopment())
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

            services.AddServices<object>();

            //services.AddTransient<IUserStorage, UserStorage>();
            //services.AddTransient<IBoardStorage, BoardStorage>();
            //services.AddTransient<IColumnStorage, ColumnStorage>();


            //services.AddTransient<IAuthService, AuthService>();
            //services.AddTransient<IBoardService, BoardService>();
            //services.AddTransient<IColumnService, ColumnService>();
            //services.AddTransient<ITaskService, TaskService>();

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
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<UserAvatarContext>();
                context?.Database.MigrateAsync();
                EnsureAdminCreated(context);
            }

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserAvatar v1"));

            app.UseRouting();
            
            // Use cors
            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()); 

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void EnsureAdminCreated(UserAvatarContext context)
        {
            var adminUser = context.Users.Any(x=> x.Email == "admin@admin.com" 
                                                  && x.Login == "admin");
            if (!adminUser)
            {
                context.Users.Add(new User
                {
                    Email = "admin@admin.com",
                    Login = "admin",
                    PasswordHash = PasswordHash.CreateHash("admin"),
                    Role = "admin",
                });
            }

            context.SaveChanges();
        }
    }
}
