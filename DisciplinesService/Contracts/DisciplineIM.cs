using System.ComponentModel.DataAnnotations;

namespace DisciplinesService.Contracts
{
    /// <summary>
    ///     Входные данные по дисциплине
    /// </summary>
    public class DisciplineIM
    {
        /// <summary>
        ///     Наименование
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Код
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        ///     Аннотация к дисциплине
        /// </summary>
        public string Annotion { get; set; }
    }
}
