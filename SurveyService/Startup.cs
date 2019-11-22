using AutoMapper;
using Common.Infrastructure;
using Common.Infrastructure.Guarantors;
using Common.Infrastructure.RabbitMQ;
using Common.Infrastructure.RabbitMQ.ConfigurationSettings;
using Common.Model.DB.ConfigurationSettings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SurveyService.Extensions;
using SurveyService.Infrastructure.BackgroundServices;
using SurveyService.Models.Db;
using SurveyService.Services.Disciplines;
using SurveyService.Services.DisciplinesService;
using SurveyService.Services.Tests;
using System;
using System.Collections.Generic;

namespace SurveyService
{
    public partial class Startup
    {
        private readonly IOptions<DbContextSettings> _dbContextSettings;
        private readonly IOptions<RabbitMqSettings> _rabbitmqSettings;
        private readonly string _rootPath;

        /// <summary>
        ///     Создание экземпляра <seealso cref="Startup"/>.
        /// </summary>
        /// <param name="configuration">Конфигуарция приложения</param>
        /// <param name="env">Окружение</param>
        /// <param name="dbContextSettings">Параметры dbContext</param>
        /// <param name="rabbitmqSettings">Параметры rabbitmq</param>
        public Startup(IConfiguration configuration,
            IHostingEnvironment env,
            IOptions<DbContextSettings> dbContextSettings,
            IOptions<RabbitMqSettings> rabbitmqSettings)
        {
            Configuration = configuration;
            _rootPath = env.ContentRootPath;
            _dbContextSettings = dbContextSettings;
            _rabbitmqSettings = rabbitmqSettings;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Entity Framework 
            services.AddDbContext<SurveysDbContext>(options =>
                options
                    //.EnableSensitiveDataLogging()
                    .UseNpgsql(
                        _dbContextSettings.Value.ConnectionString,
                        b => b.MigrationsAssembly("SurveyService")
                    )
            );

            //guarantors
            RegisterGuarantors(services);

            //automapper
            services.AddAutoMapper(typeof(SurveysProfile));

            //httpServices
            services
                .AddHttpService<DisciplinesService, DisciplinesHttpServiceSettings>((opt, client) => { client.BaseAddress = new Uri(opt.BaseUrl); })
                .AddHttpService<TestsService, TestsHttpServiceSettings>((opt, client) => { client.BaseAddress = new Uri(opt.BaseUrl); });

            //hostedServices
            services.AddHostedService<ExternalServicesChangesHandlerBS>();

            //swagger
            services.AddSwaggerGen(ConfigureSwaggerGen);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private void RegisterGuarantors(IServiceCollection services)
        {
            services.AddSingleton<IStartupPreConditionGuarantor, RabbitMqStartupPreConditionGuarantor>(x => new RabbitMqStartupPreConditionGuarantor(_rabbitmqSettings.Value, new List<RabbitMqBinding>
            {
                new RabbitMqBinding {
                    Exchange = new ExchangeSettings("devfestdisciplines", ExchangeType.Topic, true),
                    Queue = ExternalServicesChangesHandlerBS.Queue,
                    RoutingKey = $"disciplines.*"
                },
                new RabbitMqBinding {
                    Exchange = new ExchangeSettings("devfesttests", ExchangeType.Topic, true),
                    Queue = ExternalServicesChangesHandlerBS.Queue,
                    RoutingKey = $"tests.*"
                }

            }))
                .AddSingleton<IStartupPreConditionGuarantor, DbContextSchemaPreConditionGuarantor<SurveysDbContext>>(x => new DbContextSchemaPreConditionGuarantor<SurveysDbContext>(_dbContextSettings.Value.AutoMigrate));
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
