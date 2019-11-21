using SurveyService.Extensions;
using SurveyService.Services.Contracts;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SurveyService.Services.Disciplines
{
    /// <summary>
    ///     Сервис работы с дисциплинами
    /// </summary>
    public class DisciplinesService: HttpService
    {
        /// <summary>
        ///     Создание экземпляра класса <see cref="DisciplinesService"/>
        /// </summary>
        /// <param name="client">Http client</param>
        public DisciplinesService(HttpClient client) : base(client)
        {
        }

        private const string DisciplinesUrl = "/api/disciplines";

        /// <summary>
        ///     Получение списка дисциплин.
        /// </summary>
        /// <exception cref="ExternalHttpServiceException">В случае ошибки при отправке запроса внешний сервис или его обработки</exception>
        /// <returns>Дисциплины</returns>
        public async Task<IEnumerable<ExternalDisciplineDTO>> GetDisciplinesAsync()
        {
            HttpResponseMessage res = null;
            try
            {
                res = await _client.GetAsync(DisciplinesUrl);
            }
            catch (HttpRequestException e)
            {
                throw new ExternalHttpServiceException(e);
            }
            if (res.IsSuccessStatusCode)
            {
                return await res.ReadContentAsync<IEnumerable<ExternalDisciplineDTO>>();
            }
            else
                throw new ExternalHttpServiceException((int)res.StatusCode, res.ReasonPhrase);
        }

        /// <summary>
        ///     Получение дисциплины.
        /// </summary>
        /// <param name="id">Идентификатор дисциплины</param>
        /// <exception cref="ExternalHttpServiceException">В случае ошибки при отправке запроса внешний сервис или его обработки</exception>
        /// <exception cref="NotFoundException">В случае если сущность не была найдена</exception>
        /// <returns>Дисциплина</returns>
        public async Task<ExternalDisciplineDTO> GetDisciplineByIdAsync(long id)
        {
            HttpResponseMessage res = null;
            try
            {
                res = await _client.GetAsync($"{DisciplinesUrl}/{id}");
            }
            catch (HttpRequestException e)
            {
                throw new ExternalHttpServiceException(e);
            }
            if (res.IsSuccessStatusCode)
            {
                return await res.ReadContentAsync<ExternalDisciplineDTO>();
            }

            if (res.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException(typeof(ExternalDisciplineDTO), id);
            throw new ExternalHttpServiceException((int)res.StatusCode, res.ReasonPhrase);
        }
    }
}
