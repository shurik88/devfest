using System;
using System.Collections.Generic;

namespace Common.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Настройки очереди
    /// </summary>
    public class QueueSettings
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="QueueSettings"/>
        /// </summary>
        /// <param name="name">Название очереди</param>
        public QueueSettings(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            Durable = true;
            Exclusive = false;
            AutoDelete = false;
            Arguments = new Dictionary<string, object>();
            Name = name;
        }

        /// <summary>
        /// Является ли очередь постоянной или временной 
        /// Если false, то после перезапуска брокера она будет удалена, иначе - нет
        /// По умолчанию true
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// Является ли очередь эксклюзивной
        /// Если да, то она используется только одним соединением, и будет автоматически удалена при закрытии соединения
        /// По умолчанию false
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// Признак автоматического удаления
        /// Если true, то если у очереди есть хотя бы один подписчик, то после удаления последнего, очередь будет удалена
        /// По умочланию false
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// Прочие спецефические настройки
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Название очереди
        /// Должно быть уникальным
        /// </summary>
        public string Name { get; private set; }
    }
}
