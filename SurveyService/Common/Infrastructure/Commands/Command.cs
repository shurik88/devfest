using System;

namespace Common.Infrastructure.Commands
{
    /// <summary>
    /// Команда
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Название команды
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="services">Контекст зависимостей</param>
        /// <exception cref="CommandExecutionException">При ошибках</exception>
        public abstract void Execute(IServiceProvider services);
    }
}
