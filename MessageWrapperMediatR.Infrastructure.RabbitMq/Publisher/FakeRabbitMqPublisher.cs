using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Publisher
{
    public class FakeRabbitMqPublisher : IRabbitMqPublisher
    {
        public Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeRabbitMq<T> message)
        {
            throw new NotImplementedException();
        }
    }
}
