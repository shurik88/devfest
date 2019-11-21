using System.IO;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Common.Startup.Extensions
{
    /// <summary>
    ///     Методы расширения для <seealso cref="IWebHostBuilder" />.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        ///     Включение логирования.
        /// </summary>
        /// <param name="builder">построитель веб хоста приложения</param>
        /// <param name="logsPath">Путь сохранения логов</param>
        /// <returns>построитель веб хоста приложени</returns>
        public static IWebHostBuilder ConfigureBaseLogging(this IWebHostBuilder builder, string logsPath = null)
        {
            return builder.ConfigureLogging((hostingContext, logging) =>
            {
                var loggingConfiguration = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Logger(l => l
                        .MinimumLevel.Warning()
                        .Filter.ByExcluding(x =>
                            x.Properties.ContainsKey("AuthorizationFilter") && x.MessageTemplate.Text ==
                            "Authorization failed for the request at filter '{AuthorizationFilter}'.")
                        .WriteTo.RollingFile(Path.Combine(
                            logsPath ?? Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "logs"),
                            "log-{Date}.txt"))
                        .WriteTo.LiterateConsole());

                if (hostingContext.HostingEnvironment.IsDevelopment())
                    loggingConfiguration.MinimumLevel.Debug();
                Log.Logger = loggingConfiguration.CreateLogger();
                logging.AddSerilog();
            });
        }
    }
}