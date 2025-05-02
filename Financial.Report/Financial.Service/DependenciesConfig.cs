using Financial.Service.Interfaces;
using Financial.Service.Works;
using Microsoft.Extensions.DependencyInjection;

namespace Financial.Service
{
    public static class DependenciesConfig
    {
        public static IServiceCollection AddRespositoriDependecie(this IServiceCollection services)
        {
                     

            return services;
        }

        public static IServiceCollection AddServicesDependecie(this IServiceCollection services)
        {

            services.AddTransient<IFinanciallaunchService, FinanciallaunchService>(); 


            return services;
        }

        public static IServiceCollection AddBackgroundServiceDependecie(this IServiceCollection services)
        {

            services.AddHostedService<FinanciallaunchBackgroundService>(); // Register your worker


            return services;
        }
    }
}
