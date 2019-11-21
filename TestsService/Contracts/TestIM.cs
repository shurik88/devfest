using System.Collections.Generic;

namespace TestsService.Contracts
{
    /// <summary>
    ///     Тест
    /// </summary>
    public class TestIM
    {
        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Автор теста
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Признак удаления
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     Тест содержит креативную часть.
        /// </summary>
        public bool HasCreativePart { get; set; }

        /// <summary>
        ///     Вопросы
        /// </summary>
        public ICollection<QuestionIM> Questions { get; set; }
    }
}
