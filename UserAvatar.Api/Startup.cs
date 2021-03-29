﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Api.Extensions;
using UserAvatar.Bll.Gamification.Services;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Dal.Context;

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
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<LimitationOptions>(Configuration.GetSection("Limitations"))
                .AddDbContexts(Configuration)
                .AddServices()
                .AddStorages()
                .AddHostedService<ScoreTransactionBus>()
                .AddHostedService<DailyQuestsHostedService>();
            
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
            });

            
            services.AddAuthentications();
            services.AddHealthChecks();
            services.AddSwagger();
            
            services.AddCors();
            
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserAvatarContext userAvatarContext)
        {
            userAvatarContext?.Database.Migrate();
            SeedingExtension.PopulateDatabase(userAvatarContext);
            
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
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
