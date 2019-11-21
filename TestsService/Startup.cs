using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TestsService.DataAccess;
using TestsService.Extensions;
using TestsService.Settings;

namespace TestsService
{
    public partial class Startup
    {
        private readonly IOptions<MongoDbSettings> _mongoSettings;
        private readonly string _rootPath;
        /// <summary>
        ///     Создание экземпляра <seealso cref="Startup"/>.
        /// </summary>
        /// <param name="configuration">Конфигуарция приложения</param>
        /// <param name="env">Окружение</param>
        /// <param name="mongoSettings">Параметры mongo</param>
        public Startup(IConfiguration configuration,
            IHostingEnvironment env,
            IOptions<MongoDbSettings> mongoSettings)
        {
            Configuration = configuration;
            _rootPath = env.ContentRootPath;
            _mongoSettings = mongoSettings;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //automapper
            services.AddAutoMapper(typeof(TestsProfile));

            services.AddTransient<MongoTestRepository>(x => { return new MongoTestRepository(x.GetRequiredService<IOptions<MongoDbSettings>>().Value.Connection); });
            //swagger
            services.AddSwaggerGen(ConfigureSwaggerGen);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseBaseSwagger();

            app.UseMvc();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("unhandled request!");
            });
        }
    }
}
