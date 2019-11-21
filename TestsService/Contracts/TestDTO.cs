using System;

namespace TestsService.Contracts
{
    /// <summary>
    ///     Тест
    /// </summary>
    public class TestDTO
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
        ///     Тест является утвержденным
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        ///     Дата создания
        /// </summary>
        public DateTime Created { get; set; }
    }
}
