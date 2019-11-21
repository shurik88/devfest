namespace Common.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Привязка очереди к обменнику
    /// </summary>
    public class RabbitMqBinding
    {
        /// <summary>
        /// Очередь
        /// </summary>
        public QueueSettings Queue { get; set; }

        /// <summary>
        /// Обменик
        /// </summary>
        public ExchangeSettings Exchange { get; set; }

        /// <summary>
        /// Ключ маршрутизации
        /// </summary>
        public string RoutingKey { get; set; }
    }
}