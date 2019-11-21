using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestsService.Models
{
    /// <summary>
    ///     Тест
    /// </summary>
    public class Test
    {
        /// <summary>
        ///     Создание экземпляра класса <see cref="Test"/>
        /// </summary>
        public Test()
        {
            Questions = new Collection<Question>();
        }
        /// <summary>
        ///     Идентифактор
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        ///     Тест является утвержденным
        /// </summary>
        public bool IsApproved { get; set; }

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
        ///     Дата создания
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        ///     Тест содержит креативную часть.
        /// </summary>
        public bool HasCreativePart { get; set; }

        /// <summary>
        ///     Вопросы
        /// </summary>
        public ICollection<Question> Questions { get; set; }
    }
}
