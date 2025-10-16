using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.Kafka.Publisher
{
    public class FakeKafkaPublisher : IKafkaPublisher
    {
        public Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeKafka<T> message)
        {
            throw new NotImplementedException();
        }
    }
}
