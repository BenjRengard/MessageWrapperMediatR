using MessageWrapperMediatR.Infrastructure.Kafka.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageWrapperMediatR.Infrastructure.Kafka
{
    public static class InfrastructureKafkaServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            KafkaConfig kafkaConfig = configuration.GetSection("KafkaConfig").Get<KafkaConfig>() ?? new KafkaConfig();
            _ = services.AddSingleton(kafkaConfig);
            _ = services.AddSingleton<IKafkaPublisher, KafkaPublisher>();
            return services;
        }
    }
}
