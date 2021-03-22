using System;
using System.Linq;
using AutoMapper;
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
using UserAvatar.Api.Extentions;
using UserAvatar.API.Extentions;
using UserAvatar.Api.Options;
using UserAvatar.Bll.Services;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;


namespace UserAvatar.API
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
            services.AddAutoMapper(c=>c.AddProfile<MappingProfile>(), typeof(Startup));
            services.AddServices();
            
            services.AddDbContext<UserAvatarContext>(
                    options =>
                        options.UseNpgsql(
                            Configuration.GetConnectionString("connectionString"),
                            x => x.MigrationsAssembly("UserAvatar.DAL")), ServiceLifetime.Transient);
            
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
            
            services.AddCors();
            
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


        }
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<UserAvatarContext>();
                context?.Database.Migrate();
                EnsureAdminCreated(context);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserAvatar v1"));
            }
            
            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Authorization", "Accept", "Content-Type", "Origin"));
            
            app.UseMiddleware<LoggingMiddleware>();
            
            app.UseRouting();
            

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
