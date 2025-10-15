using MessageWrapperMediatR.Infrastructure.IbmMqSeries.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries
{
    public static class InfrastructureIbmMqSeriesServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageSystem(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetSection("MqSeriesConfig") is MqSeriesConfig mqSeriesConfig)
            {
                _ = services.AddSingleton(mqSeriesConfig);
                _ = services.AddSingleton<IMqSeriesPublisher, MqSeriesPublisher>();
            }
            return services;
        }
    }
}
