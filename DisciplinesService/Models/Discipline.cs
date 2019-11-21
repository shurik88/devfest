namespace DisciplinesService.Models
{
    /// <summary>
    ///     Дисциплина
    /// </summary>
    public class Discipline
    {
        /// <summary>
        ///     Идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Признак удаления.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Аннотация к дисциплине
        /// </summary>
        public string Annotion { get; set; }
    }
}
