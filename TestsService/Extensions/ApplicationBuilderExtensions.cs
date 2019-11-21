using Microsoft.AspNetCore.Builder;

namespace TestsService.Extensions
{
    /// <summary>
    ///     Методы расширения для <seealso cref="IApplicationBuilder" />.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     Подключение swagger.
        /// </summary>
        /// <param name="app">Построитель канала обработки запросов</param>
        public static void UseBaseSwagger(this IApplicationBuilder app)
        {
            const string docsRoute = "api-docs";
            app.UseSwagger(x =>
            {
                x.RouteTemplate = $"{docsRoute}/{{documentName}}/swagger.json";

                x.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });
            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(x =>
            {
                x.RoutePrefix = docsRoute;
                x.SwaggerEndpoint("v1/swagger.json", "Devfest API v1");
            });
        }
    }
}
