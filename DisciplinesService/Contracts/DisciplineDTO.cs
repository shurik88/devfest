namespace DisciplinesService.Contracts
{
    /// <summary>
    ///     Дисциплина
    /// </summary>
    public class DisciplineDTO
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
        ///     Код
        /// </summary>
        public string Code { get; set; }
    }
}
