using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.Kafka.Publisher
{
    public class PublisherMessageEnveloppeKafka<T> : PublisherMessageEnveloppeBase<T>
    {
        public string Topic { get; private set; }

        public PublisherMessageEnveloppeKafka(T message, string messageType, string topic, string messageId = null) : base(message, messageType, messageId)
        {
            this.Topic = topic;
        }
    }
}
