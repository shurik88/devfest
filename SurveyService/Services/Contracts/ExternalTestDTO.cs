using System.Collections.Generic;

namespace SurveyService.Services.Contracts
{
    /// <summary>
    ///     Тест
    /// </summary>
    public class ExternalTestDTO
    {
        /// <summary>
        ///     Идентифактор
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Вопросы
        /// </summary>
        public IEnumerable<ExternalTestQuestionDTO> Questions { get; set; }
    }
}
