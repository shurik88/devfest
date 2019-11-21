using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Common.Infrastructure.RabbitMQ.ConfigurationSettings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Слушатель очереди RabbitMQ
    /// </summary>
    public class RabbitMqConsumer
    {
        private readonly ConnectionFactory _factory;
        private readonly ILogger<RabbitMqConsumer> _logger;

        /// <summary>
        /// Создание экземпляра класс <seealso cref="RabbitMqConsumer"/>
        /// </summary>
        /// <param name="rabbitSettings">Настройки rabbitmq</param>
        /// <param name="logger">Логгер</param>
        public RabbitMqConsumer(IOptions<RabbitMqSettings> rabbitSettings, ILogger<RabbitMqConsumer> logger) : this(rabbitSettings.Value, logger)
        {
        }

        /// <summary>
        /// Создание экземпляра класс <seealso cref="RabbitMqConsumer"/>
        /// </summary>
        /// <param name="rabbitSettings">Настройки rabbitmq</param>
        /// <param name="logger">Логгер</param>
        public RabbitMqConsumer(RabbitMqSettings rabbitSettings, ILogger<RabbitMqConsumer> logger)
        {
            _factory = new ConnectionFactory()
            {
                Uri = new Uri(rabbitSettings.Connection)
            };
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Отписать от очереди
        /// </summary>
        /// <param name="consumerTag">Идентификатор слушателя</param>
        public void UnSubScribeQueue(string consumerTag)
        {
            if (string.IsNullOrEmpty(consumerTag))
                throw new ArgumentNullException(nameof(consumerTag));
            if (_subscribeChannel == null)
                throw new ArgumentNullException(nameof(_subscribeChannel), "Consumer is not subscribed");

            _subscribeChannel.BasicCancel(consumerTag);
            _subscribeChannel.Dispose();
            _subscribeConnection.Dispose();
        }

        private IConnection _subscribeConnection;
        private IModel _subscribeChannel;

        /// <summary>
        /// Подписаться на сообщения очереди
        /// </summary>
        /// <param name="queue">Настройки очереди</param>
        /// <param name="proccessMessage">Обработка сообщения</param>
        /// <param name="cancellationToken">Токен отмены задачи</param>
        /// <returns>Идентификатор подписчика. Необходим, чтобы отписаться</returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public string SubscribeQueue(QueueSettings queue, Func<string, Task<RabbitMqMessageProcessingResult>> proccessMessage, CancellationToken cancellationToken)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (proccessMessage == null)
                throw new ArgumentNullException(nameof(proccessMessage));
            _subscribeConnection = _factory.CreateConnection();
            _subscribeChannel = _subscribeConnection.CreateModel();
            var consumer = new EventingBasicConsumer(_subscribeChannel);
            consumer.Received += async (ch, ea) =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (ea != null)
                {
                    var message = Encoding.UTF8.GetString(ea.Body);
                    try
                    {
                        var result = await proccessMessage(message);
                        if (result.IsSuccessfull)
                            _subscribeChannel.BasicAck(ea.DeliveryTag, false);
                        else
                        {
                            _logger.LogWarning($"Message: {message} processing failed with reason: {result.Error}");
                            //TODO: решить, что делать с сообщением
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Message: {message} failed. Message not acked");
                    }

                }
            };
            return _subscribeChannel.BasicConsume(queue.Name, false, consumer);
        }
    }
}
