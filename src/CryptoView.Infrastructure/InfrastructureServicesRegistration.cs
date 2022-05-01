using CryptoView.Core.Interfaces;
using CryptoView.Core.Interfaces.Repositories;
using CryptoView.Infrastructure.Mocks.Repositories;
using CryptoView.Infrastructure.Mocks.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoView.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            services.AddTransient<IExchangeConnectionService, MockExchangeConnectionService>();
            //TODO change to AddTransient
            services.AddSingleton<IApiRepository, MockApiRepository>();
            services.AddSingleton<IExchangeRepository, MockExchangeRepository>();
            return services;
        }
    }
}
