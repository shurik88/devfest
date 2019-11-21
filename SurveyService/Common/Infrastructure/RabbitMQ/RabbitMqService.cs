using Microsoft.Extensions.Options;
using Common.Infrastructure.RabbitMQ.ConfigurationSettings;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;

namespace Common.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Сервис по работе с RabbitMQ через AMQP
    /// </summary>
    public class RabbitMqService
    {
        private readonly ConnectionFactory _factory;

        /// <summary>
        /// Создание экземпляра класса <seealso cref="RabbitMqSettings"/>
        /// </summary>
        /// <param name="rabbitSettings">Настройки соединения</param>
        public RabbitMqService(IOptions<RabbitMqSettings> rabbitSettings) : this(rabbitSettings.Value)
        {
        }

        /// <summary>
        /// Создание экземпляра класса <seealso cref="RabbitMqSettings"/>
        /// </summary>
        /// <param name="rabbitSettings">Настройки соединения</param>
        public RabbitMqService(RabbitMqSettings rabbitSettings)
        {
            if(rabbitSettings == null)
                throw new ArgumentNullException(nameof(rabbitSettings));
            _factory = new ConnectionFactory()
            {
                Uri = new Uri(rabbitSettings.Connection)
            };

        }

        /// <summary>
        /// Привязать очередь к обменику
        /// </summary>
        /// <param name="binding">Описание привязки</param>
        public void BindQueueToExchange(RabbitMqBinding binding)
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    DeclareExchange(channel, binding.Exchange);
                    DeclareQueue(channel, binding.Queue);
                    channel.QueueBind(binding.Queue.Name, binding.Exchange.Name, binding.RoutingKey ?? "");
                }
            }
        }

        /// <summary>
        ///  Отправка данных в обменник
        /// </summary>
        /// <param name="data">Данные для отправки</param>
        /// <param name="exchange">Настройки обменика</param>
        public void PushToExchange(string data, ExchangeSettings exchange)
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    DeclareExchange(channel, exchange);
                    BasicPublish(channel, exchange.Name, data);
                }
            }
        }

        /// <summary>
        /// Проверка доступности сервера
        /// </summary>
        /// <param name="settings">Настройки сервера rabbitmq</param>
        /// <returns>True-удалось соединиться, иначе false</returns>
        public static bool CheckServerAvailability(RabbitMqSettings settings)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(settings.Connection)
            };

            try
            {
                using (factory.CreateConnection())
                {
                }
            }
            catch (BrokerUnreachableException)
            {
                return false;
            }
            return true;
        }

        private static void BasicPublish(IModel channel, string exchangeName, string data)
        {
            var body = Encoding.UTF8.GetBytes(data);
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
        }

        /// <summary>
        /// Создание обменика
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="settings">Параметры обменика</param>
        private static void DeclareExchange(IModel channel, ExchangeSettings settings)
        {
            channel.ExchangeDeclare(settings.Name, settings.StringType, settings.Durable);
        }

        /// <summary>
        /// Создание очереди
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="settings">Настройки очереди</param>
        private static void DeclareQueue(IModel channel, QueueSettings settings)
        {
            channel.QueueDeclare(settings.Name, settings.Durable, settings.Exclusive, settings.AutoDelete, settings.Arguments);
        }
    }
}
