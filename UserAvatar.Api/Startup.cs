using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Api.Extentions;
using UserAvatar.Bll.Gamification.Services;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;

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
            services
                .Configure<LimitationOptions>(Configuration.GetSection("Limitations"));
            
            services.AddHealthChecks();
            services.AddServices();
            services.AddStorages();
            
            services.AddAuthentications();
            
            services.AddDbContexts(Configuration);

            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
            });
            
            services.AddHostedService<ScoreTransactionBus>();

            services.AddSwagger();
            
            services.AddCors();
            
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserAvatarContext userAvatarContext)
        {
            userAvatarContext?.Database.Migrate();
            
            SeedingExtension.EnsureAdminCreated(userAvatarContext);
            SeedingExtension.EnsureEventsCreated(userAvatarContext);
            SeedingExtension.EnsureRanksCreated(userAvatarContext);
            
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
