using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scraper.Domain;
using Scraper.Infrastructure;
using Scrapper.Service;
using System.Threading.Tasks;

namespace Scraper.Daemon
{
    class Program
    {
        static Task Main(string[] args)
        {
            return CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {

                    services.AddDbContext<ScraperContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("ScraperContext")));

                    var hostSection = context.Configuration.GetSection("HostUrl");
                    services.AddSingleton(hostSection.Value);
                    services.AddScoped<IShowRepository, ShowRepository>();
                    
                    services.AddHostedService<ScrapperService>();

                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });

        }

    }
}
