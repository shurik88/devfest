using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Linq;

namespace DisciplinesService
{
    public partial class Startup
    {
        private static void ConfigureSwaggerGen(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
            {
                Version = "v1",
                Title = "DevFest",
                Description = "DevFest Disciplines service API Reference",
                TermsOfService = "None",
                Contact = new Contact { Url = "https://devfest.gdgastra.ru/" }
            });
            options.DescribeAllEnumsAsStrings();
            options.CustomSchemaIds(x => x.FullName);

            //Set the comments path for the swagger json and ui.
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            options.IncludeXmlComments(Path.Combine(basePath, "DisciplinesService.xml"));

            options.OrderActionsBy(api => api.RelativePath);

            #region Security definitions

            //AddSecurityDefinition(options, JwtBearerDefaults.AuthenticationScheme, "Bearer authentication (Bearer)");

            #endregion

            options.OperationFilter<ProducesOperationFilter>();
        }


        /// <summary>
        /// Фильтр для назначения операциям возвращаемых типов содержимого (produces) на основе атрибутов <see cref="ProducesAttribute"/>
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
