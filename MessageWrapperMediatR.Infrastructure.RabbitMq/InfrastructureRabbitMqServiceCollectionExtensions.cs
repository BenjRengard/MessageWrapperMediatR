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
            RabbitConfig? rabbitConfig = configuration.GetSection("RabbitConfig").Get<RabbitConfig>();
            if (rabbitConfig == null)
            {
                rabbitConfig = new RabbitConfig();
                _ = services.AddSingleton<IRabbitChannelReceiveFactory, FakeRabbitChannelReceiveFactory>();
                _ = services.AddSingleton<IRabbitAdministrationFactory, FakeRabbitAdministrationFactory>();
                _ = services.AddSingleton<IRabbitChannelPublishFactory, FakeRabbitChannelPublishFactory>();
                _ = services.AddSingleton<IRabbitMqPublisher, FakeRabbitMqPublisher>();
            }
            else
            {
                _ = services.AddSingleton<IRabbitChannelReceiveFactory, RabbitChannelReceiveFactory>();
                _ = services.AddSingleton<IRabbitAdministrationFactory, RabbitAdministrationFactory>();
                _ = services.AddSingleton<IRabbitChannelPublishFactory, RabbitChannelPublishFactory>();
                _ = services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
            }
            _ = services.AddSingleton(rabbitConfig);

            return services;
        }
    }
}
