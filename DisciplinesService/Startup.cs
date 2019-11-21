using AutoMapper;
using DisciplinesService.Extensions;
using DisciplinesService.Models.Db;
using DisciplinesService.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DisciplinesService
{
    public partial class Startup
    {
        private readonly IOptions<DbContextSettings> _dbContextSettings;
        private readonly string _rootPath;

        /// <summary>
        ///     Создание экземпляра <seealso cref="Startup"/>.
        /// </summary>
        /// <param name="configuration">Конфигуарция приложения</param>
        /// <param name="env">Окружение</param>
        /// <param name="dbContextSettings">Параметры dbContext</param>
        public Startup(IConfiguration configuration,
            IHostingEnvironment env,
            IOptions<DbContextSettings> dbContextSettings)
        {
            Configuration = configuration;
            _rootPath = env.ContentRootPath;
            _dbContextSettings = dbContextSettings;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Entity Framework 
            services.AddDbContext<DisciplinesDbContext>(options =>
                options
                    .EnableSensitiveDataLogging()
                    .UseNpgsql(
                        _dbContextSettings.Value.ConnectionString,
                        b => b.MigrationsAssembly("DisciplinesService")
                    )
            );

            //automapper
            services.AddAutoMapper(typeof(DisciplinesProfile));

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
