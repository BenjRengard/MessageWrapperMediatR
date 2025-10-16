using MessageWrapperMediatR.Infrastructure.Kafka.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageWrapperMediatR.Infrastructure.Kafka
{
    public static class InfrastructureKafkaServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            KafkaConfig? kafkaConfig = configuration.GetSection("KafkaConfig").Get<KafkaConfig>();
            if (kafkaConfig == null)
            {
                kafkaConfig = new KafkaConfig();
                _ = services.AddSingleton<IKafkaPublisher, FakeKafkaPublisher>();
            }
            else
            {
                _ = services.AddSingleton<IKafkaPublisher, KafkaPublisher>();
            }
            _ = services.AddSingleton(kafkaConfig);
            return services;
        }
    }
}
