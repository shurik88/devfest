using System;

namespace Common.Infrastructure
{
    /// <summary>
    /// Исключение, вызванное неправилньой средой запуска приложения.
    /// Дальнешее выпонление невозможно.
    /// </summary>
    public class StartupPreConditionException : Exception
    {
        /// <summary>
        /// Создание экземпляра класса <seealso cref="StartupPreConditionException"/>.
        /// </summary>
        /// <param name="message">Сообщение</param>
        public StartupPreConditionException(string message) : base(message)
        {
        }
    }
}
