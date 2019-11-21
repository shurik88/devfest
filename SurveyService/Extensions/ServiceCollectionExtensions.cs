using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SurveyService.Services;
using System;
using System.Net.Http;

namespace SurveyService.Extensions
{
    /// <summary>
    ///     Методы расширения для DI контейнера
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Регистрация http service
        /// </summary>
        /// <typeparam name="TService">тип сервиса</typeparam>
        /// <typeparam name="TOptions">тип настроеек</typeparam>
        /// <param name="services">Контепйнер DI</param>
        /// <param name="configuration">Конфигурация Http клиента на основе</param>
        /// <returns>Контейнер DI</returns>
        public static IServiceCollection AddHttpService<TService, TOptions>(this IServiceCollection services, Action<TOptions, HttpClient> configuration)
            where TService : HttpService
            where TOptions : class, new()
        {
            services.AddSingleton<TService>();
            ConfigureHttpClient<TService, TOptions>(services, configuration);
            return services;
        }

        private static void ConfigureHttpClient<TService, TOptions>(IServiceCollection services, Action<TOptions, HttpClient> configuration)
            where TService : HttpService
            where TOptions : class, new()
        {
            services.AddHttpClient<TService>((container, client) =>
            {
                var options = container.GetRequiredService<IOptions<TOptions>>();
                configuration(options.Value, client);
            });
        }

        /// <summary>
        ///     Регистрация http service
        /// </summary>
        /// <typeparam name="TInterface">тип интерфейса сервиса</typeparam>
        /// <typeparam name="TService">тип сервиса</typeparam>
        /// <typeparam name="TOptions">тип настроеек</typeparam>
        /// <param name="services">Контепйнер DI</param>
        /// <param name="configuration">Конфигурация Http клиента на основе</param>
        /// <returns>Контейнер DI</returns>
        public static IServiceCollection AddHttpService<TInterface, TService, TOptions>(this IServiceCollection services, Action<TOptions, HttpClient> configuration)
            where TInterface : class
            where TService : HttpService, TInterface
            where TOptions : class, new()
        {
            services.AddSingleton<TInterface, TService>();
            ConfigureHttpClient<TService, TOptions>(services, configuration);
            return services;
        }
    }
}
