using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries.Publisher
{
    public class PublisherMessageEnveloppeMqSeries<T> : PublisherMessageEnveloppeBase<T>
    {
        public string Queue { get; private set; }

        public PublisherMessageEnveloppeMqSeries(T message, string messageType, string queue, string messageId = null)
            : base(message, messageType, messageId)
        {
            this.Queue = queue;
        }
    }
}
