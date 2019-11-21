using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MosRegTest.IndependingTesting.Web.Common.Startup.Extensions
{
    /// <summary>
    ///     Методы расширения для конфигурации swagger.
    /// </summary>
    public static class SwaggerExtensions
    {
        //private static readonly string OKResponseDescription = "Успешно";
        private static readonly string UnauthorizedResponseDescription = "Ошибка авторизации";
        private static readonly string ForbiddenResponseDescription = "Доступ запрещен";

        /// <summary>
        ///     Конфигурация swagger базовая для всех проектов.
        /// </summary>
        /// <param name="options">Параметры настройки swagger</param>
        /// <param name="apiDescription">Описание api</param>
        /// <param name="mainAssemblyDocsName">Название основной сборки</param>
        public static void BaseConfigure(this SwaggerGenOptions options, string apiDescription,
            string mainAssemblyDocsName)
        {
            options.SwaggerDoc("v1", new Info
            {
                Version = "v1",
                Title = "Abbyy",
                Description = apiDescription,
                TermsOfService = "None",
                Contact = new Contact {Url = "http://abbyy.com"}
            });
            options.DescribeAllEnumsAsStrings();

            //Set the comments path for the swagger json and ui.
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            options.IncludeXmlComments(Path.Combine(basePath, mainAssemblyDocsName));

            options.OrderActionsBy(api => api.RelativePath);

            options.OperationFilter<ProducesOperationFilter>();
            //options.OperationFilter<FileResultOperationFilter>();
            options.OperationFilter<SecurityRequirementsOperationFilter>();
            //options.OperationFilter<FileUploadOperationWorkaroundFilter>();
        }


        /// <summary>
        ///     Подключение безопасности для swagger.
        /// </summary>
        /// <param name="options">Параметры настройки swagger</param>
        /// <param name="authenticationScheme">Схема аутентификации</param>
        /// <param name="description">Описание</param>
        public static void AddSecurityDefinition(this SwaggerGenOptions options, string authenticationScheme,
            string description)
        {
            options.AddSecurityDefinition(authenticationScheme, new ApiKeyScheme
            {
                Type = "apiKey",
                In = "header",
                Name = "Authorization",
                Description = description
            });
        }

        /// <summary>
        ///     Фильтр для назначения операциям схем безопасности и генерирования описаний ответов с кодами 401 и 403, если они
        ///     отсутствуют в описании.
        /// </summary>
        private class SecurityRequirementsOperationFilter : IOperationFilter
        {
            /// <inheritdoc />
            public void Apply(Operation operation, OperationFilterContext context)
            {
                var authenticationSchemes = context
                    .ApiDescription
                    .ActionDescriptor
                    .FilterDescriptors
                    .Select(x => x.Filter)
                    .OfType<AuthorizeFilter>()
                    .SelectMany(x => x.Policy?.AuthenticationSchemes ?? Enumerable.Empty<string>()).ToList();

                if (!authenticationSchemes.Any())
                    return;
                if (!operation.Responses.ContainsKey("401"))
                    operation.Responses.Add("401", new Response {Description = UnauthorizedResponseDescription});
                if (!operation.Responses.ContainsKey("403"))
                    operation.Responses.Add("403", new Response {Description = ForbiddenResponseDescription});

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
                foreach (var authenticationScheme in authenticationSchemes)
                    operation.Security.Add(
                        new Dictionary<string, IEnumerable<string>>
                        {
                            {authenticationScheme, Enumerable.Empty<string>()}
                        });
            }
        }

        /// <summary>
        ///     Фильтр для назначения операциям возвращаемых типов содержимого (produces) на основе атрибутов
        ///     <seealso cref="ProducesAttribute" />.
        /// </summary>
        private class ProducesOperationFilter : IOperationFilter
        {
            public void Apply(Operation operation, OperationFilterContext context)
            {
                var producesFilters = context
                    .ApiDescription
                    .ActionDescriptor
                    .FilterDescriptors
                    .Select(x => x.Filter)
                    .OfType<ProducesAttribute>();

                foreach (var contentType in producesFilters.SelectMany(x => x.ContentTypes))
                    operation.Produces.Add(contentType);
            }
        }
    }
}