using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.Infrastructure;
using Common.Infrastructure.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Common.Startup.Extensions
{
    /// <summary>
    ///     Методы расширения для работы с <seealso cref="IWebHost" />.
    /// </summary>
    public static class WebHostExtensions
    {
        /// <summary>
        ///     Запуск гаранторов.
        /// </summary>
        /// <param name="webHost">Веб-хост</param>
        /// <exception cref="StartupPreConditionException">Если работа одного из гаранторов завершилась неудачно</exception>
        public static void AssertGuarantors(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<IStartupPreConditionGuarantor>>();
                try
                {
                    logger.LogInformation("Startup guarantors started");
                    foreach (var guarantor in scope.ServiceProvider.GetServices<IStartupPreConditionGuarantor>())
                        guarantor.Ensure(scope.ServiceProvider);
                    logger.LogInformation("Startup guarantors executed successfuly");
                }
                catch (StartupPreConditionException)
                {
                    logger.LogError("Startup guarantors failed");
                    throw;
                }
            }
        }

        /// <summary>
        /// Запуск команд
        /// </summary>
        /// <param name="webHost">Веб-хост</param>
        /// <param name="factories">Фабрики команд</param>
        /// <param name="args">Аргументы командной строки</param>
        /// <exception cref="CommandExecutionException">Если выполнение команды завершилось неудачно</exception>
        /// <returns>0 - команда выпонлена, 1 - некорректные аргументы, 2 - команда не была найдена</returns>
        public static int RunCommands(this IWebHost webHost, IEnumerable<ICommandLineFactory> factories, string[] args)
        {
            var factoriesDict = factories.ToDictionary(x => x.Verb);
            using (var scope = webHost.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<CommandLineApplication>>();
                var application = new CommandLineApplication(factoriesDict, scope.ServiceProvider);
                application.DisplayCommandsList();
                if (!application.CommandExists(args))
                    return 2;
                try
                {
                    return application.Run(args);
                }
                catch (CommandExecutionException e)
                {
                    logger.LogError("Command execution failed failed", e);
                    throw;
                }
            }
        }
    }
}