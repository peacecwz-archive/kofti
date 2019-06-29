using System;
using Kofti.Models;
using Kofti.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kofti
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddKofti(this IServiceCollection services)
        {
            services.AddSingleton<IConfigService, ConfigService>();
            return services;
        }

        public static void LoadConfig(this IApplicationBuilder applicationBuilder)
        {
            var configService = applicationBuilder.ApplicationServices.GetRequiredService<IConfigService>();
            configService.Load();
        }

        public static void InitKoftiClient(this IApplicationBuilder applicationBuilder)
        {
            var configService = applicationBuilder.ApplicationServices.GetRequiredService<IConfigService>();
            configService.InitClient();
        }

        public static void InitKoftiServer(this IApplicationBuilder applicationBuilder, Action<KoftiInitMessage> action)
        {
            var configService = applicationBuilder.ApplicationServices.GetRequiredService<IConfigService>();
            configService.InitServer(action);
        }
    }
}