using Microsoft.EntityFrameworkCore;
using SurveyService.Models.Subordinates;

namespace SurveyService.Models.Db
{
    /// <summary>
    ///     Контекст бд анкетирование
    /// </summary>
    public class SurveysDbContext : DbContext
    {
        // <summary>
        ///     Создание экземпляра класса <see cref="DisciplinesSurveysDbContext"/>.
        /// </summary>
        /// <param name="options">Параметры подключения контекста</param>
        public SurveysDbContext(DbContextOptions<SurveysDbContext> options) : base(options)
        {
        }

        /// <summary>
        ///     Дисциплины
        /// </summary>
        public DbSet<Discipline> Disciplines { get; set; }

        /// <summary>
        ///     Дисциплины
        /// </summary>
        public DbSet<Test> Tests { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Discipline>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.Property(x => x.IsDeleted).IsRequired();
            });

            modelBuilder.Entity<Test>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.Property(x => x.QuestionsCount).IsRequired();
                e.Property(x => x.IsDeleted).IsRequired();
                e.Property(x => x.ExternalId).IsRequired();
                e.HasIndex(x => x.ExternalId).IsUnique();
            });

            //modelBuilder.HasDefaultSchema("disciplines");

            base.OnModelCreating(modelBuilder);
        }
    }
}
