namespace DisciplinesService.Contracts
{
    /// <summary>
    ///     Дисциплина
    /// </summary>
    public class DisciplineDetailsDTO : DisciplineDTO
    {
        /// <summary>
        ///     Аннотация к дисциплине
        /// </summary>
        public string Annotion { get; set; }
    }
}
