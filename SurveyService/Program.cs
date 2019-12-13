using System.Collections.Generic;
using System.IO;
using Common.Infrastructure.Commands;
using Common.Infrastructure.RabbitMQ.ConfigurationSettings;
using Common.Model.DB.ConfigurationSettings;
using Common.Startup.Extensions;
using Infrastructure.Commands.Sync;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Serilog;
using SurveyService.Services.DisciplinesService;
using SurveyService.Services.Tests;

namespace SurveyService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            try
            {
                host.AssertGuarantors();
                var res = host.RunCommands(new List<ICommandLineFactory> { new SyncCommandFactory() }, args);
                if (res != 2)
                    return;

                host.Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hosting, config) => { ConfigureAppConfiguration(hosting, config, args); })
                .ConfigureServices(ConfigureAppServices)
                .ConfigureLogging(ConfigureLogging)
                .UseStartup<Startup>();

        private static void ConfigureLogging(WebHostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            logging.SetMinimumLevel(LogLevel.Error);
            var logsPath = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "logs");
            var loggingConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Logger(l => l
                    .MinimumLevel.Error()
                    .WriteTo.RollingFile(Path.Combine(logsPath, "log-{Date}.txt"))
                    .WriteTo.LiterateConsole());
            //if (hostingContext.HostingEnvironment.IsDevelopment())
            //    loggingConfiguration.MinimumLevel.Debug();
            //Log.Logger = loggingConfiguration.CreateLogger();
            logging.AddFilter(x => x == LogLevel.Error);
            logging.AddSerilog(loggingConfiguration.CreateLogger());
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
                .Configure<DbContextSettings>(configuration.GetSection("database"))
                .Configure<DisciplinesHttpServiceSettings>(configuration.GetSection("services:disciplines"))
                .Configure<TestsHttpServiceSettings>(configuration.GetSection("services:tests"))
                .Configure<RabbitMqSettings>(configuration.GetSection("rabbitmq"));
        }
    }
}
