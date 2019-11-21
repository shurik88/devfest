using System.Collections.Generic;

namespace TestsService.Contracts
{
    /// <summary>
    ///     Вопрос
    /// </summary>
    public class QuestionDTO
    {
        /// <summary>
        ///   Текст вопроса  
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Признак обязательности
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        ///     Разрешен выбор нескольких вариантов
        /// </summary>
        public bool HasMany { get; set; }

        /// <summary>
        ///     Врианты ответа
        /// </summary>
        public IEnumerable<string> Choices { get; set; }
    }
}