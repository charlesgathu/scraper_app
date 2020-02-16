using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                .ConfigureAppConfiguration((context, config) =>
                {
                    //config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                    //config.AddEnvironmentVariables();

                })
                .ConfigureServices((context, services) =>
                {
                    //var settings = new Dictionary<string, object> {
                    //    { "variable1", context.Configuration.GetValue<string>("myapp_variable1") },
                    //    { "variable2", context.Configuration.GetValue<string>("myapp_variable2") },
                    //    { "variable3", context.Configuration.GetValue<string>("myapp_variable3") }
                    //};
                    //services.AddSingleton(settings);
                    services.AddHostedService<MyHostedService>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });

        }

    }
}
