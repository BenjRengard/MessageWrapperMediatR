using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.Kafka.Publisher
{
    public interface IKafkaPublisher
    {
        Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeKafka<T> message);
    }
}
