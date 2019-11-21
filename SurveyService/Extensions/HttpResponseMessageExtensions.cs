using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SurveyService.Services;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SurveyService.Extensions
{
    /// <summary>
    ///     Методы расширения для работы с <seealso cref="HttpResponseMessage"/>
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        ///     Чтение контента из ответа.
        /// </summary>
        /// <typeparam name="TContract">тип данных, которые будут прочитаны</typeparam>
        /// <param name="httpResponceMessage">Сообщение http-ответа</param>
        /// <exception cref="ExternalHttpServiceException">В случае, если не удалось распарсить ответ</exception>
        /// <returns>Данные</returns>
        public static async Task<TContract> ReadContentAsync<TContract>(this HttpResponseMessage httpResponceMessage)
            where TContract : class
        {
            var responceBody = await httpResponceMessage.Content.ReadAsStreamAsync();
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy HH:mm:ss" });
            using (var reader = new StreamReader(responceBody))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    try
                    {
                        return serializer.Deserialize<TContract>(jsonReader);
                    }
                    catch (JsonReaderException ex)
                    {
                        throw new ExternalHttpServiceException(ex);
                    }
                }
            }
        }
    }
}
