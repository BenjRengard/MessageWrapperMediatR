using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Publisher
{
    public interface IRabbitMqPublisher
    {
        Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeRabbitMq<T> message);
    }
}