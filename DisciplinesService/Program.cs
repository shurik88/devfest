using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DisciplinesService.Settings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DisciplinesService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hosting, config) => { ConfigureAppConfiguration(hosting, config, args); })
                .ConfigureServices(ConfigureAppServices)
                .ConfigureLogging(ConfigureLogging)
                .UseStartup<Startup>();

        private static void ConfigureLogging(WebHostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            var logsPath = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "logs");
            var loggingConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Logger(l => l
                    .MinimumLevel.Warning()
                    .WriteTo.RollingFile(Path.Combine(logsPath, "log-{Date}.txt"))
                    .WriteTo.LiterateConsole());
            if (hostingContext.HostingEnvironment.IsDevelopment())
                loggingConfiguration.MinimumLevel.Debug();
            Log.Logger = loggingConfiguration.CreateLogger();
            logging.AddSerilog();
        }

        private static void ConfigureAppConfiguration(WebHostBuilderContext hosting, IConfigurationBuilder config, string[] args)
        {
            var env = hosting.HostingEnvironment;

            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            //if (env.IsDevelopment())
            //    config.AddUserSecrets<Startup>();
        }

        private static void ConfigureAppServices(WebHostBuilderContext hostingContext, IServiceCollection services)
        {
            var configuration = hostingContext.Configuration;
            services.AddOptions()
                .Configure<DbContextSettings>(configuration.GetSection("database"));
        }
    }
}
