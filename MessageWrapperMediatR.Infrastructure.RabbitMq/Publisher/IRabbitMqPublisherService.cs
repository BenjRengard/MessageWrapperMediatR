using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Publisher
{
    public interface IRabbitMqPublisherService
    {
        Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeRabbitMq<T> message);
    }
}