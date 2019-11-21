namespace Common.Model.DB.ConfigurationSettings
{
    /// <summary>
    /// Настройки подключения к DbContext
    /// </summary>
    public class DbContextSettings
    {
        /// <summary>
        /// Строка подключения к бд
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Выполнять ли автоматическую миграцию схемы бд
        /// </summary>
        public bool AutoMigrate { get; set; }
    }
}
