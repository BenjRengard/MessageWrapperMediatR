using MessageWrapperMediatR.Infrastructure.Kafka.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageWrapperMediatR.Infrastructure.Kafka
{
    public static class InfrastructureKafkaServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetSection("KafkaConfig") is KafkaConfig kafkaConfig)
            {
                _ = services.AddSingleton(kafkaConfig);
                _ = services.AddSingleton<IKafkaPublisher, KafkaPublisher>();
            }
            return services;
        }
    }
}
