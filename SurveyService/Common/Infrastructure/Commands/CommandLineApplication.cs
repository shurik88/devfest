using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Commands
{
    /// <summary>
    /// Контейнер команд приложения
    /// </summary>
    internal class CommandLineApplication
    {
        private readonly IDictionary<string, ICommandLineFactory> _commandlineFactories;
        private readonly IServiceProvider _services;
        private readonly ILogger<CommandLineApplication> _logger;

        /// <summary>
        /// Создание экземпляра класса <see cref="CommandLineApplication"/>
        /// </summary>
        /// <param name="commandsFactories"></param>
        /// <param name="services">Контейнер DI</param>
        public CommandLineApplication(IDictionary<string, ICommandLineFactory> commandsFactories, IServiceProvider services)
        {
            if (commandsFactories == null || !commandsFactories.Any())
                throw new ArgumentNullException(nameof(commandsFactories));

            if (commandsFactories.Any(x => string.IsNullOrEmpty(x.Key)))
                throw new ArgumentException("One or many empty keys in commandline factories");

            _commandlineFactories = commandsFactories;
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = services.GetService<ILogger<CommandLineApplication>>();
        }

        /// <summary>
        /// Проверка возможности запуска команды из аргументов командной строки
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        /// <returns>Да/нет</returns>
        public bool CommandExists(string[] args) => _commandlineFactories.Keys.Any(args.Contains);

        /// <summary>
        /// Выпорлнение команды согласно аргументам командной строки
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        /// <exception cref="CommandExecutionException">Ошибка выполнения команды</exception>
        /// <returns>0 - выполнилась, 1 - не удалось распарсить аргументы или была запрошена справка, 2 - команда не была найдена</returns>
        public int Run(string[] args)
        {
            (var factory, var commandArgs) = ParseArgs(args);

            if (factory == null)
                return 2;

            return RunCommand(factory, commandArgs);
        }

        private (ICommandLineFactory Factory, string[] CommandArgs) ParseArgs(string[] args)
        {
            for (var i = 0; i < args.Length; ++i)
            {
                if (_commandlineFactories.ContainsKey(args[i]))
                {
                    return (_commandlineFactories[args[i]], args.Skip(i + 1).ToArray());
                }
            }
            return (null, null);
        }

        /// <summary>
        /// Отображение списка команд
        /// </summary>
        public void DisplayCommandsList() => Console.WriteLine($"Available commands: {string.Join(", ", _commandlineFactories.Keys)}");


        private int RunCommand(ICommandLineFactory factory, string[] args)
        {

            if (args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine(factory.HelpMessage);
                return 1;
            }
            var command = factory.Create(args);
            if (command == null)
            {
                Console.WriteLine($"Invalid agruments. Please see help for command: {factory.HelpMessage}");
                return 1;
            }

            try
            {
                Console.WriteLine($"Command: '{command.Title}' executing");
                command.Execute(_services);
                Console.WriteLine($"Command: '{command.Title}' executed successfully");
                return 0;
            }
            catch (CommandExecutionException e)
            {
                _logger.LogWarning($"Command: '{command.Title}' execution failed with error: {e.Message}");
                return 1;
            }
#pragma warning disable AV1210 // generic exception
            catch (Exception exception)
            {
                Console.WriteLine($"Command: '{command.Title}' failed with error: {exception.Message}");
                throw new CommandExecutionException("Command execution failed", exception);
            }
#pragma warning restore AV1210

        }

    }
}
