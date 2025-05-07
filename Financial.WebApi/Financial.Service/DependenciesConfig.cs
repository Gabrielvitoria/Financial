using Financial.Infra.Interfaces;
using Financial.Infra.Repositories;
using Financial.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Financial.Service
{
    [ExcludeFromCodeCoverage]
    public static class DependenciesConfig
    {

        public static IServiceCollection AddRespositoriDependecie(this IServiceCollection services)
        {

            services.AddTransient<IProcessLaunchRepository, ProcessLaunchRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddServicesDependecie(this IServiceCollection services)
        {
            services.AddTransient<IProcessLaunchservice, ProcessLaunchservice>();            
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<INotificationEvent, NotificationEventService>();
            services.AddTransient<IConnectionFactoryWrapper, ConnectionFactoryWrapper>();

            return services;
        }



    }
}
