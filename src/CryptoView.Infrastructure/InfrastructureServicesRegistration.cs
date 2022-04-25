using CryptoView.Core.Interfaces;
using CryptoView.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoView.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            services.AddTransient<IExchangeConnectionService, MoqExchangeConnectionService>();
            return services;
        }
    }
}
