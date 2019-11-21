namespace Infrastructure.Commands.Sync
{
    /// <summary>
    ///     Параметры команды синхронизации.
    /// </summary>
    public class SyncCommandArguments
    {
        /// <summary>
        ///     Принудительное обновление базы данных.
        /// </summary>
        public bool Force { get; set; }
    }
}
