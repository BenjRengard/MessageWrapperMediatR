using MessageWrapperMediatR.Infrastructure.RabbitMq.Connections;
using MessageWrapperMediatR.Infrastructure.RabbitMq.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq
{
    public static class InfrastructureRabbitMqServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetSection("RabbitConfig") is RabbitConfig rabbitConfig)
            {
                _ = services.AddSingleton(rabbitConfig);
                _ = services.AddSingleton<IRabbitChannelReceiveFactory, RabbitChannelReceiveFactory>();
                _ = services.AddSingleton<IRabbitAdministrationFactory, RabbitAdministrationFactory>();
                _ = services.AddSingleton<IRabbitChannelPublishFactory, RabbitChannelPublishFactory>();
                _ = services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
            }

            return services;
        }
    }
}
