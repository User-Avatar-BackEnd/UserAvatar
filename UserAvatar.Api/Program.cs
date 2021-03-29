using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace UserAvatar.Api
{
    /// <summary>
    /// Main method
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Program main entry method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
