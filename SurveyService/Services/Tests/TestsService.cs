using SurveyService.Extensions;
using SurveyService.Services.Contracts;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SurveyService.Services.Tests
{
    /// <summary>
    ///     Сервис тестов
    /// </summary>
    public class TestsService : HttpService
    {
        /// <summary>
        ///     Создание экземпляра класса <see cref="TestsService"/>
        /// </summary>
        /// <param name="client">Http client</param>
        public TestsService(HttpClient client) : base(client)
        {
        }

        private const string TestsUrl = "/api/tests";

        /// <summary>
        ///     Получение списка тестов
        /// </summary>
        /// <exception cref="ExternalHttpServiceException">В случае ошибки при отправке запроса внешний сервис или его обработки</exception>
        /// <returns>Дисциплины</returns>
        public async Task<IEnumerable<ExternalTestDTO>> GetTestsAsync()
        {
            HttpResponseMessage res = null;
            try
            {
                res = await _client.GetAsync(TestsUrl);
            }
            catch (HttpRequestException e)
            {
                throw new ExternalHttpServiceException(e);
            }
            if (res.IsSuccessStatusCode)
            {
                return await res.ReadContentAsync<IEnumerable<ExternalTestDTO>>();
            }
            else
                throw new ExternalHttpServiceException((int)res.StatusCode, res.ReasonPhrase);
        }

        /// <summary>
        ///     Получение теста
        /// </summary>
        /// <param name="id">Идентификатор теста</param>
        /// <exception cref="ExternalHttpServiceException">В случае ошибки при отправке запроса внешний сервис или его обработки</exception>
        /// <exception cref="NotFoundException">В случае если сущность не была найдена</exception>
        /// <returns>Дисциплина</returns>
        public async Task<ExternalTestDTO> GetTestByIdAsync(string id)
        {
            HttpResponseMessage res = null;
            try
            {
                res = await _client.GetAsync($"{TestsUrl}/{id}");
            }
            catch (HttpRequestException e)
            {
                throw new ExternalHttpServiceException(e);
            }
            if (res.IsSuccessStatusCode)
            {
                return await res.ReadContentAsync<ExternalTestDTO>();
            }

            if (res.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException(typeof(ExternalTestDTO), id);
            throw new ExternalHttpServiceException((int)res.StatusCode, res.ReasonPhrase);
        }
    }
}
