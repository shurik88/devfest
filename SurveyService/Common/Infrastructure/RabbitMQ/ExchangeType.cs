namespace Common.Infrastructure.RabbitMQ
{
    /// <summary>
    ///     Тип обменика.
    /// </summary>
    public enum ExchangeType
    {
        /// <summary>
        ///     Широковещательная передача.
        /// </summary>
        Fanout,

        /// <summary>
        ///     Точка-точка.
        /// </summary>
        Direct,

        /// <summary>
        ///     Широковещателньая по теме.
        /// </summary>
        Topic
    }
}
