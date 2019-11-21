using Newtonsoft.Json;
using System;

namespace SurveyService.Services
{
    /// <summary>
    ///     Исключение, вызванное отсутствием запрашиваемой сущности в системе.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        ///     Создание экземпляра класса <seealso cref="NotFoundException" />.
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        public NotFoundException(Type entityType) : base($"Absent {entityType.Name}")
        {
            EntityType = entityType;
        }

        /// <summary>
        ///     Создание экземпляра класса <seealso cref="NotFoundException" />
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="id">Ид сущности</param>
        public NotFoundException(Type entityType, object id) : base(
            $"{entityType.Name}:{JsonConvert.SerializeObject(id)}")
        {
            EntityType = entityType;
            Id = id;
        }

        /// <summary>
        ///     Тип сущности.
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        ///     Ид сущности.
        /// </summary>
        public object Id { get; }
    }
}
