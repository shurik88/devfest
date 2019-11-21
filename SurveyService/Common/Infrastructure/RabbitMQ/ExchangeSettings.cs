using System;

namespace Common.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Настройки подключения к обменнику
    /// </summary>
    public class ExchangeSettings
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="ExchangeSettings"/>
        /// </summary>
        /// <param name="name">Название обменика</param>
        /// <param name="type">Тип обменика</param>
        /// <param name="durable">Постоянное или временное хранилище</param>
        public ExchangeSettings(string name, ExchangeType type = ExchangeType.Fanout, bool durable = true)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Type = type;
            Durable = durable;
        }

        /// <summary>
        /// Тип обменика
        /// </summary>
        public string StringType
        {
            get
            {
                switch (Type)
                {
                    case ExchangeType.Fanout:
                        return "fanout";
                    case ExchangeType.Direct:
                        return "direct";
                    case ExchangeType.Topic:
                        return "topic";
                    default:
                        throw new ArgumentOutOfRangeException(Type.ToString());
                }
            }
        }

        /// <summary>
        /// Название обменика
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Тип обменика
        /// </summary>
        public ExchangeType Type { get; private set; }

        /// <summary>
        /// Если true, то обменик восстанавливается после перезапуска брокера, иначе не восстанавливается
        /// </summary>
        public bool Durable { get; private set; }
    }
}
