using Common.Infrastructure.RabbitMQ;
using Common.Infrastructure.RabbitMQ.ConfigurationSettings;
using Infrastructure.Commands.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SurveyService.Models.Db;
using SurveyService.Models.Subordinates;
using SurveyService.Services;
using SurveyService.Services.Disciplines;
using SurveyService.Services.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyService.Infrastructure.BackgroundServices
{
    /// <summary>
    ///     Слушатель изменений внешних сервисов
    /// </summary>
    public class ExternalServicesChangesHandlerBS : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly RabbitMqConsumer _consumer;

        /// <summary>
        ///     Создание экземпляра класса <see cref="ExternalServicesChangesHandlerBS"/>
        /// </summary>
        /// <param name="services">Контейнер DI</param>
        /// <param name="settings">Параметры rabbitmq</param>
        public ExternalServicesChangesHandlerBS(
            IServiceProvider services,
            IOptions<RabbitMqSettings> settings)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _consumer = new RabbitMqConsumer(settings?.Value ?? throw new ArgumentNullException(nameof(settings)), services.GetRequiredService<ILogger<RabbitMqConsumer>>());
        }

        /// <summary>
        ///     Очередь, которую слушает фоновый сервис.
        /// </summary>
        public static readonly QueueSettings Queue = new QueueSettings("surveys");

        //TODO: Если логика разрастется, вынести в отдельные классы
        private readonly IDictionary<Tuple<string, string>, 
            Func<IServiceScope, EntityChange, Task<RabbitMqMessageProcessingResult>>> _handlers = 
            new Dictionary<Tuple<string, string>, Func<IServiceScope, EntityChange, Task<RabbitMqMessageProcessingResult>>>
        {
            [new Tuple<string, string>("disciplines", "disciplines")] = HandleDisciplineTableAsync,
            [new Tuple<string, string>("tests", "tests")] = HandleTestTableAsync
        };

        /// <inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = Task.Run(() =>
            {
                var consumerTag = _consumer.SubscribeQueue(Queue, ReadMessageAsync, stoppingToken);
                stoppingToken.WaitHandle.WaitOne();
                _consumer.UnSubScribeQueue(consumerTag);
            }, stoppingToken);
            return task;
        }

        private async Task<RabbitMqMessageProcessingResult> ReadMessageAsync(string message)
        {
            using (var scope = _services.CreateScope())
            {
                var data = JsonConvert.DeserializeObject<EntityChange>(message);
                var key = new Tuple<string, string>(data.Source, data.Entity);
                if (!_handlers.ContainsKey(key))
                    return RabbitMqMessageProcessingResult.Fail("too many messages");

                var handler = _handlers[key];
                return await handler(scope, data);
            }
        }

        #region tests
        private static async Task<RabbitMqMessageProcessingResult> HandleTestTableAsync(IServiceScope scope, EntityChange change)
        {
            var service = scope.ServiceProvider.GetRequiredService<TestsService>();
            var context = scope.ServiceProvider.GetRequiredService<SurveysDbContext>();
            var id = change.EntityKey;
            switch (change.OperationType)
            {
                case OperationType.Insert:
                case OperationType.Update:
                    {
                        try
                        {
                            var res = await service.GetTestByIdAsync(id);
                            var test = context.Tests.FirstOrDefault(x => x.ExternalId == id);
                            if (test == null)
                            {
                                test = new Test { ExternalId = id };
                                context.Tests.Add(test);
                            }
                            SyncCommand.ReplaceTest(test, res);
                            await context.SaveChangesAsync();
                        }
                        catch (NotFoundException)
                        {
                            await DeleteTestAsync(context, id);
                        }
                        return RabbitMqMessageProcessingResult.Success;

                    }
                case OperationType.Delete:
                    await DeleteTestAsync(context, id);
                    return RabbitMqMessageProcessingResult.Success;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change.OperationType), change.OperationType, "unknown value");
            }
        }

        private static async Task DeleteTestAsync(SurveysDbContext context, string id)
        {
            var testToDelete = await context.Tests.FirstOrDefaultAsync(x => x.ExternalId == id);
            if (testToDelete != null)
            {
                testToDelete.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region disciplines
        private static async Task<RabbitMqMessageProcessingResult> HandleDisciplineTableAsync(IServiceScope scope, EntityChange change)
        {
            var service = scope.ServiceProvider.GetRequiredService<DisciplinesService>();
            var context = scope.ServiceProvider.GetRequiredService<SurveysDbContext>();
            var id = long.Parse(change.EntityKey);
            switch (change.OperationType)
            {
                case OperationType.Insert:
                case OperationType.Update:
                    {
                        try
                        {

                            var res = await service.GetDisciplineByIdAsync(id);
                            var discipline = context.Disciplines.FirstOrDefault(x => x.Id == id);
                            if (discipline == null)
                            {
                                discipline = new Discipline { Id = id };
                                context.Disciplines.Add(discipline);
                            }
                            SyncCommand.ReplaceDiscipline(discipline, res);
                            await context.SaveChangesAsync();
                        }
                        catch (NotFoundException)
                        {
                            await DeleteDisciplineAsync(context, id);
                        }
                        return RabbitMqMessageProcessingResult.Success;

                    }
                case OperationType.Delete:
                    await DeleteDisciplineAsync(context, id);
                    return RabbitMqMessageProcessingResult.Success;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change.OperationType), change.OperationType, "unknown value");
            }
        }

        private static async Task DeleteDisciplineAsync(SurveysDbContext context, long id)
        {
            var disciplineToDelete = await context.Disciplines.FirstOrDefaultAsync(x => x.Id == id);
            if (disciplineToDelete != null)
            {
                disciplineToDelete.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }
        #endregion

        private class EntityChange
        {
            /// <summary>
            ///     Название таблицы
            /// </summary>
            [JsonProperty("entity")]
            public string Entity { get; set; }

            /// <summary>
            ///     Название сервиса
            /// </summary>
            [JsonProperty("source")]
            public string Source { get; set; }

            /// <summary>
            ///     Тип операции
            /// </summary>
            [JsonProperty("operation")]
            [JsonConverter(typeof(StringEnumConverter))]
            public OperationType OperationType { get; set; }

            /// <summary>
            ///     Тип операции.
            /// </summary>
            [JsonProperty("key")]
            public string EntityKey { get; set; }
        }

        private enum OperationType
        {
            /// <summary>
            ///  Entity Update
            /// </summary>
            Update,

            /// <summary>
            ///  Entity Insert
            /// </summary>
            Insert,

            /// <summary>
            ///  Enttiy Delete.
            /// </summary>
            Delete
        }
    }
}
