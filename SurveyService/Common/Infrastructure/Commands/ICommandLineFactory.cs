namespace Common.Infrastructure.Commands
{
    /// <summary>
    ///Фабрика создания экземпляра команды
    /// </summary>
    public interface ICommandLineFactory
    {
        /// <summary>
        /// Справка по команде
        /// </summary>
        string HelpMessage { get; }

        /// <summary>
        /// Имя команды
        /// </summary>
        string Verb { get; }

        /// <summary>
        /// Создание экземпляра команды
        /// </summary>
        /// <remarks>
        /// Если команду, не удалось создать, то будет возвращен null
        /// </remarks>
        /// <param name="args">Аргументы командной строки</param>
        /// <returns></returns>
        Command Create(string[] args);
    }
}
