using Financial.Infra;
using Financial.Infra.Interfaces;
using Financial.Service.Interfaces;
using Financial.Service.Works;
using Microsoft.Extensions.DependencyInjection;

namespace Financial.Service
{
    public static class DependenciesConfig
    {
        public static IServiceCollection AddRespositoriDependecie(this IServiceCollection services)
        {

            services.AddTransient<IFinanciallaunchRespository, FinanciallaunchRespository>();

            return services;
        }

        public static IServiceCollection AddServicesDependecie(this IServiceCollection services)
        {

            services.AddTransient<IFinanciallaunchService, FinanciallaunchService>();

            return services;
        }

        public static IServiceCollection AddServiceExternal(this IServiceCollection services, string connetionStringRedis)
        {

            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = connetionStringRedis;
                option.InstanceName = "FinancialInstance";
            });

            return services;
        }


        public static IServiceCollection AddBackgroundServiceDependecie(this IServiceCollection services)
        {

            services.AddHostedService<FinanciallaunchBackgroundService>();
            services.AddHostedService<FinancialPayLaunchBackgroundService>();


            return services;
        }
    }
}
