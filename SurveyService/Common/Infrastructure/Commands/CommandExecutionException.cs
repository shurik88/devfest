using System;

namespace Common.Infrastructure.Commands
{
    /// <summary>
    /// Исключение при выполнении команды <seealso cref="Command"/>
    /// </summary>
    public class CommandExecutionException : Exception
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="CommandExecutionException"/>
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public CommandExecutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="CommandExecutionException"/>
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="innerException">Внутреннее исключение</param>
        public CommandExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
