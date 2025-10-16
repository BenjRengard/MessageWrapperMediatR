using MessageWrapperMediatR.Infrastructure.IbmMqSeries.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries
{
    public static class InfrastructureIbmMqSeriesServiceCollectionExtensions
    {
        public static IServiceCollection AddIbmMqSeriesConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            MqSeriesConfig? mqSeriesConfig = configuration.GetSection("MqSeriesConfig").Get<MqSeriesConfig>();
            if (mqSeriesConfig == null)
            {
                mqSeriesConfig = new MqSeriesConfig();
                _ = services.AddSingleton<IMqSeriesPublisher, MqSeriesPublisher>();
            }
            else
            {
                _ = services.AddSingleton<IMqSeriesPublisher, MqSeriesPublisher>();
            }
            _ = services.AddSingleton(mqSeriesConfig);
            return services;
        }
    }
}
