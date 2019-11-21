using System;

namespace SurveyService.Services
{
    /// <summary>
    /// Исключение при отправке запроса внешний сервис
    /// </summary>
    public class ExternalHttpServiceException : Exception
    {
        /// <summary>
        /// Создание экземпляра класса <seealso cref="ExternalHttpServiceException"/>
        /// </summary>
        /// <param name="unhandledCode">Код ответа</param>
        /// <param name="reasonPhrase">Причина</param>
        public ExternalHttpServiceException(int unhandledCode, string reasonPhrase) : base($"Unhandled responce code: {unhandledCode}")
        {
            Details = new { Code = unhandledCode, ReasonPhrase = reasonPhrase };
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="ExternalHttpServiceException"/>
        /// </summary>
        /// <param name="ex">Исключение, связанное с невозможностью отправки сообщения или чтения ответа</param>
        public ExternalHttpServiceException(Exception ex) : base("Не удалось отправить запрос или обработать ответ", ex)
        {
            Details = new { ex.Message };
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="ExternalHttpServiceException"/>
        /// </summary>
        /// <param name="message">Сообщение об ошибке при нарушении согласованности выходного формата</param>
        public ExternalHttpServiceException(string message) : base("")
        {
            Details = new { Message = message };
        }

        /// <summary>
        /// Детали ошибки
        /// </summary>
        public object Details { get; private set; }
    }
}
