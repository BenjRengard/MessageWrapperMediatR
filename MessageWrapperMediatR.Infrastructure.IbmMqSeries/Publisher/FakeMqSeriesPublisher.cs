using MessageWrapperMediatR.Infrastructure.Messaging;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries.Publisher
{
    public class FakeMqSeriesPublisher : IMqSeriesPublisher
    {
        public Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeMqSeries<T> message)
        {
            throw new NotImplementedException();
        }
    }
}
