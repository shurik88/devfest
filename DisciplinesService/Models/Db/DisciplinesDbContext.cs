using Microsoft.EntityFrameworkCore;

namespace DisciplinesService.Models.Db
{
    /// <summary>
    ///     Контекст бд дисциплин
    /// </summary>
    public class DisciplinesDbContext: DbContext
    {
        // <summary>
        ///     Создание экземпляра класса <see cref="DisciplinesDbContext"/>.
        /// </summary>
        /// <param name="options">Параметры подключения контекста</param>
        public DisciplinesDbContext(DbContextOptions<DisciplinesDbContext> options) : base(options)
        {
        }

        /// <summary>
        ///     Дисциплины
        /// </summary>
        public DbSet<Discipline> Disciplines { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Discipline>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(255);
                e.Property(x => x.Code).IsRequired();
                e.Property(x => x.IsDeleted).IsRequired();
                e.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsDeleted\" = false");
            });

            //modelBuilder.HasDefaultSchema("disciplines");

            base.OnModelCreating(modelBuilder);
        }


    }
}
