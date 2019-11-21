namespace SurveyService.Models.Subordinates
{
    /// <summary>
    ///     Тест
    /// </summary>
    public class Test
    {        
        /// <summary>
        ///     Идентифактор
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        ///     Идентифактор из внешнего сервиса
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Признак удаления
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     Количество вопросов
        /// </summary>
        public int QuestionsCount { get; set; }
    }
}
