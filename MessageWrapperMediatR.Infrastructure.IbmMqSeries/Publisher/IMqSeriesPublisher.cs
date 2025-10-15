using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries.Publisher
{
    /// <summary>
    /// Publisher for MqSeries.
    /// </summary>
    public interface IMqSeriesPublisher
    {
        /// <summary>
        /// Publish message in MqSeries Queue.
        /// </summary>
        /// <param name="message"></param>
        Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeMqSeries<T> message);
    }
}
