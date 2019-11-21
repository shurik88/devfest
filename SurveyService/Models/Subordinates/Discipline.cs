namespace SurveyService.Models.Subordinates
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
    }
}
