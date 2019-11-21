using System;
using System.Net.Http;

namespace SurveyService.Services
{
    /// <summary>
    ///     Базовый класс работы с http сервисами
    /// </summary>
    public abstract class HttpService
    {
        /// <summary>
        ///     http client
        /// </summary>
        protected readonly HttpClient _client;

        /// <summary>
        ///     Создрание экземпляра класса <see cref="HttpService"/>
        /// </summary>
        /// <param name="client">Http client</param>
        public HttpService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
}
