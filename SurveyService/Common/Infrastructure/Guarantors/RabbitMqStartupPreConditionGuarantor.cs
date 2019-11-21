using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.Infrastructure.RabbitMQ;
using Common.Infrastructure.RabbitMQ.ConfigurationSettings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Infrastructure.Guarantors
{
    /// <summary>
    /// Проверка и выполнение всех необходимых предустловий для использования RabbitMQ
    /// </summary>
    public class RabbitMqStartupPreConditionGuarantor : IStartupPreConditionGuarantor
    {
        private readonly RabbitMqSettings _settings;
        private readonly IEnumerable<RabbitMqBinding> _bindings;

        /// <summary>
        /// Создание экземпляра <see cref="RabbitMqStartupPreConditionGuarantor"/>
        /// </summary>
        /// <param name="settings">Параметры подключения к RabbbitMQ</param>
        /// <param name="bindings">Список привязок, которые необоходимо обеспечить</param>
        public RabbitMqStartupPreConditionGuarantor(RabbitMqSettings settings, IEnumerable<RabbitMqBinding> bindings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _bindings = bindings ?? throw new ArgumentNullException(nameof(bindings));

        }

        /// <inheridoc/>
        public void Ensure(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<RabbitMqStartupPreConditionGuarantor>>();
                logger.LogInformation("Checking RabbitMq Server");
                if (!RabbitMqService.CheckServerAvailability(_settings))
                {
                    logger.LogError("RabbitMQ Server is not available for settings: {0}", _settings);
                    throw new StartupPreConditionException("RabbtiMQServer is not available");
                }
                logger.LogInformation("Check RabbitMqServer is successful");

                if (_bindings.Any())
                {
                    var service = new RabbitMqService(_settings);
                    foreach (var binding in _bindings)
                        service.BindQueueToExchange(binding);
                    logger.LogInformation("RabbitMQ bindings are created");
                }
            }
        }
    }
}
