namespace Common.Infrastructure.RabbitMQ
{
    /// <summary>
    ///     Результат обработки сообщения.
    /// </summary>
    public class RabbitMqMessageProcessingResult
    {
        private RabbitMqMessageProcessingResult()
        {

        }

        /// <summary>
        ///     В случаее неуспешной обработки сообщения фиксируется код или сообщение о причине ошибки.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        ///     Признак успешной обработки сообщения.
        /// </summary>
        public bool IsSuccessfull { get; private set; }

        /// <summary>
        ///     Создание успешного результата обработки сообщения.
        /// </summary>
        public static RabbitMqMessageProcessingResult Success => new RabbitMqMessageProcessingResult { IsSuccessfull = true };

        /// <summary>
        ///     Создание неуспешного результата обработки сообщения.
        /// </summary>
        /// <param name="error">Сообщение об ошибке</param>
        public static RabbitMqMessageProcessingResult Fail(string error) => new RabbitMqMessageProcessingResult { IsSuccessfull = false, Error = error };
    }
}
