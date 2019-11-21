using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.Model.DB.Migrator;

namespace Common.Infrastructure.Guarantors
{
    /// <summary>
    ///     Проверка схем баз данных ef.
    /// </summary>
    public class DbContextSchemaPreConditionGuarantor<TContext> : IStartupPreConditionGuarantor
        where TContext : DbContext
    {
        private readonly bool _autoMigrate;

        /// <summary>
        ///     Создание экземпляра класса <seealso cref="DbContextSchemaPreConditionGuarantor{TContext}" />.
        /// </summary>
        /// <param name="autoMigrate">Выполнение отсутствующих миграций в бд автоматически</param>
        public DbContextSchemaPreConditionGuarantor(bool autoMigrate = false)
        {
            _autoMigrate = autoMigrate;
        }

        /// <inheritdoc />
        public void Ensure(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var logger = scope.ServiceProvider
                    .GetService<ILogger<DbContextSchemaPreConditionGuarantor<TContext>>>();
                var context = services.GetRequiredService<TContext>();
                var modelDifferences = context.GetModelDifferences();
                if (modelDifferences.Any())
                {
                    logger.LogError("Not all migrations were created for {@context}: {@migrations}", context.GetType(),
                        modelDifferences);
                    throw new StartupPreConditionException("Application has model-migrations differences");
                }

                var databaseDifferences = context.GetDatabaseDifferences();
                if (!databaseDifferences.Any())
                    return;

                if (_autoMigrate)
                {
                    context.Database.Migrate();
                    logger.LogInformation("Absent Migration were applied");
                }
                else
                {
                    logger.LogError("Not all migrations were applied for {@context}: {@migrations}",
                        context.GetType(), databaseDifferences);
                    throw new StartupPreConditionException("Application has database-migrations differences");
                }
            }
        }
    }
}