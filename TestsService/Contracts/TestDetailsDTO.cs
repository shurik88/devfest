using System.Collections.Generic;

namespace TestsService.Contracts
{
    /// <summary>
    ///     Тест
    /// </summary>
    public class TestDetailsDTO: TestDTO
    {
        /// <summary>
        ///     Автор теста
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Тест содержит креативную часть.
        /// </summary>
        public bool HasCreativePart { get; set; }

        /// <summary>
        ///     Вопросы
        /// </summary>
        public ICollection<QuestionDTO> Questions { get; set; }
    }
}
